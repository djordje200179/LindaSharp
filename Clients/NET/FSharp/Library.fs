module LindaSharp.Client.FSharp

open LindaSharp.Client

let inline put (linda: RemoteLinda) tuple = linda.Put tuple

let inline get (linda: RemoteLinda) tuple = linda.Get tuple

let inline query (linda: RemoteLinda) tuple = linda.Query tuple

let inline tryGet (linda: RemoteLinda) tuple = async {
    let! result = 
        tuple
        |> linda.TryGet 
        |> Async.AwaitTask

    return ValueOption.ofObj result
}

let inline tryQuery (linda: RemoteLinda) tuple = async {
    let! result = 
        tuple
        |> linda.TryQuery 
        |> Async.AwaitTask

    return ValueOption.ofObj result
}

let inline registerScript (linda: RemoteLinda) key script =
    (key, script)
    |> linda.RegisterScript
    |> Async.AwaitTask

let inline invokeScript (linda: RemoteLinda) script = 
    script
    |> linda.InvokeScript
    |> Async.AwaitTask

let inline evalScript (linda: RemoteLinda) script = 
    script
    |> linda.EvalScript
    |> Async.AwaitTask

let inline getScriptExecutionStatus (linda: RemoteLinda) id =
    id
    |> linda.GetScriptExecutionStatus
    |> Async.AwaitTask

let inline isHealthy (linda: RemoteLinda) = 
    linda
    |> _.IsHealthy()
    |> Async.AwaitTask