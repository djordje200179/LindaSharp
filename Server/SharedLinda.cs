using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System.Collections.Concurrent;

namespace LindaSharp.Server;

public class SharedLinda(IActionEvalLinda linda) : IScriptEvalLinda, ISpaceViewLinda {
	private readonly ILinda localLinda = linda;
	private readonly ScriptLocalLinda scriptLocalLinda = new(linda);

	private readonly ScriptEngine pythonEngine = Python.CreateEngine();
	private readonly ConcurrentDictionary<string, string> evalScripts = new();

	public Task Put(object[] tuple) => localLinda.Put(tuple);

	public Task<object[]> Get(object?[] pattern) => localLinda.Get(pattern);
	public Task<object[]> Query(object?[] pattern) => localLinda.Get(pattern);

	public Task<object[]?> TryGet(object?[] pattern) => localLinda.TryGet(pattern);
	public Task<object[]?> TryQuery(object?[] pattern) => localLinda.TryQuery(pattern);

	public async Task RegisterScript(string key, string ironpythonCode) {
		evalScripts[key] = ironpythonCode;
	}

	public async Task InvokeScript(string key, object? parameter = null) {
		var script = evalScripts[key];

		var scope = pythonEngine.CreateScope();
		scope.SetVariable("linda", scriptLocalLinda);
		scope.SetVariable("param", parameter);

		var thread = new Thread(() => pythonEngine.Execute(script, scope));
		thread.Start();
	}

	public async Task EvalScript(string ironpythonCode) {
		var scope = pythonEngine.CreateScope();
		scope.SetVariable("linda", scriptLocalLinda);

		var thread = new Thread(() => pythonEngine.Execute(ironpythonCode, scope));
		thread.Start();
	}

	public async Task EvalScriptFile(string ironpythonFilePath) {
		var scope = pythonEngine.CreateScope();
		scope.SetVariable("linda", scriptLocalLinda);

		var thread = new Thread(() => pythonEngine.ExecuteFile(ironpythonFilePath, scope));
		thread.Start();
	}

	public async Task<IEnumerable<object[]>> QueryAll() {
		return localLinda is ISpaceViewLinda spaceViewLinda ? await spaceViewLinda.QueryAll() : throw new NotSupportedException();
	}
}