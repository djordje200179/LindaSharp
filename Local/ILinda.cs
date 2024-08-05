namespace LindaSharp;

public interface ILinda {
	public Task Put(params object[] tuple);

	public Task<object[]> Get(params object?[] pattern);
	public Task<object[]> Query(params object?[] pattern);

	public Task<object[]?> TryGet(params object?[] pattern);
	public Task<object[]?> TryQuery(params object?[] pattern);
}

public interface IActionEvalLinda : ILinda {
	public void Eval(Action<IActionEvalLinda> func);
}

public interface IScriptEvalLinda : ILinda {
	public Task RegisterScript(string key, string ironpythonCode);
	public Task InvokeScript(string key, object? parameter = null);
	public Task EvalScript(string ironpythonCode);

	public Task RegisterScriptFile(string key, string ironpythonFilePath) => RegisterScript(key, File.ReadAllText(ironpythonFilePath));
	public Task EvalScriptFile(string ironpythonFilePath) => EvalScript(File.ReadAllText(ironpythonFilePath));
}

public interface ISpaceViewLinda : ILinda {
	public Task<IEnumerable<object[]>> QueryAll();
}