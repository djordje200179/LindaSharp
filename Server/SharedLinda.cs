using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System.Diagnostics.CodeAnalysis;

namespace LindaSharp.Server;

public interface IScriptEvalLinda : ILinda {
	public void EvalRegister(string key, string ironpythonCode);
	public void EvalRegisterFile(string key, string ironpythonFilePath) {
		var content = File.ReadAllText(ironpythonFilePath);
		EvalRegister(key, content);
	}

	public void EvalInvoke(string key, object? parameter = null);

	public void Eval(string ironpythonCode);
	public void EvalFile(string ironpythonFilePath) {
		var content = File.ReadAllText(ironpythonFilePath);
		Eval(content);
	}
}

public class SharedLinda(IActionEvalLinda linda) : IScriptEvalLinda, ISpaceViewLinda {
	private readonly ILinda localLinda = linda;
	private readonly ScriptLocalLinda scriptLocalLinda = new(linda);

	private readonly ScriptEngine pythonEngine = Python.CreateEngine();
	private readonly IDictionary<string, string> evalScripts = new Dictionary<string, string>();

	public void Out(object[] tuple) => localLinda.Out(tuple);

	public object[] In(object?[] tuplePattern) => localLinda.In(tuplePattern);
	public object[] Rd(object?[] tuplePattern) => localLinda.In(tuplePattern);

	public bool Inp(object?[] tuplePattern, [MaybeNullWhen(false)] out object[] tuple) => localLinda.Inp(tuplePattern, out tuple);
	public bool Rdp(object?[] tuplePattern, [MaybeNullWhen(false)] out object[] tuple) => localLinda.Rdp(tuplePattern, out tuple);

	public void EvalRegister(string key, string ironpythonCode) {
		evalScripts[key] = ironpythonCode;
	}

	public void EvalRegisterFile(string key, string ironpythonFilePath) {
		var content = File.ReadAllText(ironpythonFilePath);
		EvalRegister(key, content);
	}

	public void EvalInvoke(string key, object? parameter = null) {
		var script = evalScripts[key];

		var scope = pythonEngine.CreateScope();
		scope.SetVariable("linda", scriptLocalLinda);
		scope.SetVariable("param", parameter);

		var thread = new Thread(() => pythonEngine.Execute(script, scope));
		thread.Start();
	}

	public void Eval(string ironpythonCode) {
		var scope = pythonEngine.CreateScope();
		scope.SetVariable("linda", scriptLocalLinda);

		var thread = new Thread(() => pythonEngine.Execute(ironpythonCode, scope));
		thread.Start();
	}

	public void EvalFile(string ironpythonFilePath) {
		var scope = pythonEngine.CreateScope();
		scope.SetVariable("linda", scriptLocalLinda);

		var thread = new Thread(() => pythonEngine.ExecuteFile(ironpythonFilePath, scope));
		thread.Start();
	}

	public void Dispose() {
		GC.SuppressFinalize(this);
		localLinda.Dispose();
	}

	public IEnumerable<object[]> ReadAll() {
		return localLinda is ISpaceViewLinda spaceViewLinda ? spaceViewLinda.ReadAll() : throw new NotSupportedException();
	}
}