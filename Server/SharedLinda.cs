using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

namespace LindaSharp.Server;

public class SharedLinda(IActionEvalLinda linda) : IScriptEvalLinda, ISpaceViewLinda {
	private readonly ILinda localLinda = linda;
	private readonly ScriptLocalLinda scriptLocalLinda = new(linda);

	private readonly ScriptEngine pythonEngine = Python.CreateEngine();
	private readonly IDictionary<string, string> evalScripts = new Dictionary<string, string>();

	public Task Out(object[] tuple) => localLinda.Out(tuple);

	public Task<object[]> In(object?[] tuplePattern) => localLinda.In(tuplePattern);
	public Task<object[]> Rd(object?[] tuplePattern) => localLinda.In(tuplePattern);

	public Task<object[]?> Inp(object?[] tuplePattern) => localLinda.Inp(tuplePattern);
	public Task<object[]?> Rdp(object?[] tuplePattern) => localLinda.Rdp(tuplePattern);

	public async Task EvalRegister(string key, string ironpythonCode) {
		evalScripts[key] = ironpythonCode; // TODO: Fix concurrency
	}

	public async Task EvalInvoke(string key, object? parameter = null) {
		var script = evalScripts[key];

		var scope = pythonEngine.CreateScope();
		scope.SetVariable("linda", scriptLocalLinda);
		scope.SetVariable("param", parameter);

		var thread = new Thread(() => pythonEngine.Execute(script, scope));
		thread.Start();
	}

	public async Task Eval(string ironpythonCode) {
		var scope = pythonEngine.CreateScope();
		scope.SetVariable("linda", scriptLocalLinda);

		var thread = new Thread(() => pythonEngine.Execute(ironpythonCode, scope));
		thread.Start();
	}

	public async Task EvalFile(string ironpythonFilePath) {
		var scope = pythonEngine.CreateScope();
		scope.SetVariable("linda", scriptLocalLinda);

		var thread = new Thread(() => pythonEngine.ExecuteFile(ironpythonFilePath, scope));
		thread.Start();
	}

	public async Task<IEnumerable<object[]>> ReadAll() {
		return localLinda is ISpaceViewLinda spaceViewLinda ? await spaceViewLinda.ReadAll() : throw new NotSupportedException();
	}
}