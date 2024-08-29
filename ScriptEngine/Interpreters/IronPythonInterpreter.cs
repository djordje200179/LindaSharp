using IronPython.Hosting;
using ScriptHost = Microsoft.Scripting.Hosting;

namespace LindaSharp.ScriptEngine.Interpreters;

public class IronPythonInterpreter : IInterpreter {
	private readonly ScriptLocalLinda linda;
	private readonly ScriptHost.ScriptEngine pythonEngine = Python.CreateEngine();

	public IronPythonInterpreter(IActionEvalLinda linda) {
		this.linda = new ScriptLocalLinda(linda);
	}

	public Task Execute(string code, object? parameter = null) {
		var scope = pythonEngine.CreateScope();
		scope.SetVariable("linda", linda);
		scope.SetVariable("param", parameter);

		return new Task(() => pythonEngine.Execute(code, scope));
	}
}
