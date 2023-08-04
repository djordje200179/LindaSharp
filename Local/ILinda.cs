using System.Diagnostics.CodeAnalysis;

namespace LindaSharp;

public interface ILinda : IDisposable {
	public void Out(object[] tuple);

	public object[] In(object?[] tuplePattern);
	public bool Inp(object?[] tuplePattern, [MaybeNullWhen(false)] out object[] tuple);

	public object[] Rd(object?[] tuplePattern);
	public bool Rdp(object?[] tuplePattern, [MaybeNullWhen(false)] out object[] tuple);

	public void EvalRegister(string key, string pythonCode);
	public void EvalRegisterFile(string key, string pythonFilePath) {
		var content = File.ReadAllText(pythonFilePath);
		EvalRegister(key, content);
	}

	public void EvalInvoke(string key, object? parameter = null);

	public void Eval(string pythonCode);
	public void EvalFile(string pythonFilePath) {
		var content = File.ReadAllText(pythonFilePath);
		Eval(content);
	}
}