namespace LindaSharp.ScriptEngine;

public interface IInterpreter {
	Task Execute(string code, object? parameter = null);
}
