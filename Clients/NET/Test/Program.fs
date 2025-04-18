open LindaSharp.Client
open System

let rec waitForServer client =
    printfn "Waiting for server..."

    async {
        let! isHealthy = RemoteLinda.isHealthy client

        if not isHealthy then
            do! Async.Sleep 1000
            return! waitForServer client
    }

let client = new RemoteLinda("http://localhost:5001")
    
async {
    do! waitForServer client

    do! RemoteLinda.put client [| "a"; 0 |]
    do! RemoteLinda.put client [| "b"; 1 |]
    do! RemoteLinda.put client [| "ind"; 1 |]

    do! RemoteLinda.registerScriptFile client "fib-calc" "FibonachiCalculation.py"

    let! taskIds = 
        [| 0 .. 7 |]
        |> Array.map (fun _ -> RemoteLinda.invokeScript client "fib-calc")
        |> Async.Parallel

    for i in 2 .. 70 do
        let! resultTuple = RemoteLinda.get client [| Some "fib"; Some i; None |]
            
        resultTuple[2]
        :?> float
        |> int64
        |> printfn "Fib[%d] = %d" i

    do! RemoteLinda.put client [| "done" |]

    printfn "Waiting for a second..."
    do! Async.Sleep 1000

    for id in taskIds do
        let! status = RemoteLinda.getScriptExecutionStatus client id
        printfn "%d: %A" id status
}
|> Async.RunSynchronously

Console.ReadLine() |> ignore
