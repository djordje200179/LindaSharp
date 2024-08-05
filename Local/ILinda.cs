namespace LindaSharp;

public interface ILinda {
	public Task Out(object[] tuple);

	public Task<object[]> In(object?[] tuplePattern);
	public Task<object[]> Rd(object?[] tuplePattern);

	public Task<object[]?> Inp(object?[] tuplePattern);
	public Task<object[]?> Rdp(object?[] tuplePattern);
}

public interface IActionEvalLinda : ILinda {
	public void Eval(Action<IActionEvalLinda> func);
}

public interface IScriptEvalLinda : ILinda {
	public Task EvalRegister(string key, string ironpythonCode);
	public Task EvalInvoke(string key, object? parameter = null);
	public Task Eval(string ironpythonCode);

	public Task EvalRegisterFile(string key, string ironpythonFilePath) => EvalRegister(key, File.ReadAllText(ironpythonFilePath));
	public Task EvalFile(string ironpythonFilePath) => Eval(File.ReadAllText(ironpythonFilePath));
}

public interface ISpaceViewLinda : ILinda {
	public Task<IEnumerable<object[]>> ReadAll();
}