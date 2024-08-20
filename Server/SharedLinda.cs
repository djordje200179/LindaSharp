using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System.Collections.Concurrent;
using static LindaSharp.IScriptEvalLinda.ScriptExecutionStatus;

namespace LindaSharp.Server;

public class SharedLinda(IActionEvalLinda linda) : IScriptEvalLinda, ISpaceViewLinda {
	private readonly ILinda localLinda = linda;
	private readonly ScriptLocalLinda scriptLocalLinda = new(linda);

	private readonly ScriptEngine pythonEngine = Python.CreateEngine();
	private readonly ConcurrentDictionary<string, string> evalScripts = new();
	private readonly ConcurrentDictionary<int, Exception?> evalResults = new();

	public Task Put(object[] tuple) => localLinda.Put(tuple);

	public Task<object[]> Get(object?[] pattern) => localLinda.Get(pattern);
	public Task<object[]> Query(object?[] pattern) => localLinda.Get(pattern);

	public Task<object[]?> TryGet(object?[] pattern) => localLinda.TryGet(pattern);
	public Task<object[]?> TryQuery(object?[] pattern) => localLinda.TryQuery(pattern);

	public Task RegisterScript(string key, string ironpythonCode) {
		evalScripts[key] = ironpythonCode;
		return Task.CompletedTask;
	}

	private int StartScriptExecution(ScriptScope scope, string script) {
		var task = new Task(() => pythonEngine.Execute(script, scope));
		task.ContinueWith(task => evalResults[task.Id] = task.Exception);
		task.ConfigureAwait(false);
		task.Start();

		return task.Id;
	}

	public Task<int> InvokeScript(string key, object? parameter = null) {
		var script = evalScripts[key];

		var scope = pythonEngine.CreateScope();
		scope.SetVariable("linda", scriptLocalLinda);
		scope.SetVariable("param", parameter);

		return Task.FromResult(StartScriptExecution(scope, script));
	}

	public Task<int> EvalScript(string ironpythonCode) {
		var scope = pythonEngine.CreateScope();
		scope.SetVariable("linda", scriptLocalLinda);

		return Task.FromResult(StartScriptExecution(scope, ironpythonCode));
	}

	public Task<IScriptEvalLinda.ScriptExecutionStatus> GetScriptExecutionStatus(int id) {
		if (!evalResults.TryGetValue(id, out var exception))
			return Task.FromResult(new IScriptEvalLinda.ScriptExecutionStatus(ExecutionState.NotFound));

		return Task.FromResult(new IScriptEvalLinda.ScriptExecutionStatus(
			exception is null ? ExecutionState.Finished : ExecutionState.Exception,
			exception
		));
	}

	public async Task<IEnumerable<object[]>> QueryAll() {
		if (localLinda is not ISpaceViewLinda spaceViewLinda)
			throw new NotSupportedException();

		return await spaceViewLinda.QueryAll();
	}
}