using LindaSharp.ScriptEngine.Interpreters;
using System.Collections.Concurrent;
using static LindaSharp.IScriptEvalLinda.ScriptExecutionStatus;

namespace LindaSharp.ScriptEngine;

public class ScriptEvalLinda(IActionEvalLinda linda) : IScriptEvalLinda {
	private readonly ILinda localLinda = linda;
	private readonly ScriptLocalLinda scriptLocalLinda = new(linda);

	private readonly Dictionary<IScriptEvalLinda.Script.Language, IInterpreter> interpreters = new() {
		{ IScriptEvalLinda.Script.Language.IronPython, new IronPythonInterpreter(linda) }
	};

	private readonly ConcurrentDictionary<string, IScriptEvalLinda.Script> evalScripts = new();
	private readonly ConcurrentDictionary<int, Exception?> evalResults = new();

	public Task Put(object[] tuple) => localLinda.Put(tuple);

	public Task<object[]> Get(object?[] pattern) => localLinda.Get(pattern);
	public Task<object[]> Query(object?[] pattern) => localLinda.Get(pattern);

	public Task<object[]?> TryGet(object?[] pattern) => localLinda.TryGet(pattern);
	public Task<object[]?> TryQuery(object?[] pattern) => localLinda.TryQuery(pattern);

	public Task RegisterScript(string key, IScriptEvalLinda.Script script) {
		evalScripts[key] = script;
		return Task.CompletedTask;
	}

	public Task<int> InvokeScript(string key, object? parameter = null) {
		var script = evalScripts[key];
		var interpreter = interpreters[script.Type];

		var task = interpreter.Execute(script.Code, parameter);
		task.ContinueWith(task => evalResults[task.Id] = task.Exception);
		task.Start();

		return Task.FromResult(task.Id);
	}

	public Task<int> EvalScript(IScriptEvalLinda.Script script) {
		var interpreter = interpreters[script.Type];

		var task = interpreter.Execute(script.Code, null);
		task.ContinueWith(task => evalResults[task.Id] = task.Exception);
		task.Start();

		return Task.FromResult(task.Id);
	}

	public Task<IScriptEvalLinda.ScriptExecutionStatus> GetScriptExecutionStatus(int id) {
		if (!evalResults.TryGetValue(id, out var exception))
			return Task.FromResult(new IScriptEvalLinda.ScriptExecutionStatus(ExecutionState.NotFound));

		return Task.FromResult(new IScriptEvalLinda.ScriptExecutionStatus(
			exception is null ? ExecutionState.Finished : ExecutionState.Exception,
			exception
		));
	}
}