module LindaSharp.Client.RemoteLinda

open LindaSharp.Client
open LindaSharp

let inline put (linda: RemoteLinda) tuple = 
    linda.Put tuple
    |> Async.AwaitTask

let inline get (linda: RemoteLinda) tuple = 
    let tuple = Array.map Option.toObj<obj> tuple
    
    linda.Get tuple
    |> Async.AwaitTask

let inline query (linda: RemoteLinda) tuple = 
    let tuple = Array.map Option.toObj<obj> tuple
    
    linda.Query tuple
    |> Async.AwaitTask

let inline tryGet (linda: RemoteLinda) tuple = async {
    let tuple = Array.map Option.toObj<obj> tuple

    let! result = 
        linda.TryGet tuple
        |> Async.AwaitTask

    return ValueOption.ofObj result
}

let inline tryQuery (linda: RemoteLinda) tuple = async {
    let tuple = Array.map Option.toObj<obj> tuple

    let! result = 
        linda.TryQuery tuple
        |> Async.AwaitTask

    return ValueOption.ofObj result
}

let inline registerScript (linda: RemoteLinda) key script =
    (key, script)
    |> linda.RegisterScript
    |> Async.AwaitTask

let inline registerScriptFile (linda: RemoteLinda) key filePath =
    (key, filePath)
    |> linda.RegisterScriptFile
    |> Async.AwaitTask

let inline invokeScript (linda: RemoteLinda) script = 
    script
    |> linda.InvokeScript
    |> Async.AwaitTask

let inline evalScript (linda: RemoteLinda) script = 
    script
    |> linda.EvalScript
    |> Async.AwaitTask

let inline evalScriptFile (linda: RemoteLinda) scriptFile = 
    scriptFile
    |> linda.EvalScriptFile
    |> Async.AwaitTask

let inline getScriptExecutionStatus (linda: RemoteLinda) id =
    id
    |> linda.GetScriptExecutionStatus
    |> Async.AwaitTask

let inline isHealthy (linda: RemoteLinda) = 
    linda
    |> _.IsHealthy()
    |> Async.AwaitTask