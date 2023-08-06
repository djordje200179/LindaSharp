using System.Diagnostics.CodeAnalysis;

namespace LindaSharp;

public interface ILinda : IDisposable {
	public void Out(object[] tuple);

	public object[] In(object?[] tuplePattern);
	public bool Inp(object?[] tuplePattern, [MaybeNullWhen(false)] out object[] tuple);

	public object[] Rd(object?[] tuplePattern);
	public bool Rdp(object?[] tuplePattern, [MaybeNullWhen(false)] out object[] tuple);

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