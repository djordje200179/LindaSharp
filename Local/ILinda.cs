using System.Diagnostics.CodeAnalysis;

namespace LindaSharp;

public interface ILinda : IDisposable {
	public void Out(object[] tuple);

	public object[] In(object?[] tuplePattern);
	public object[] Rd(object?[] tuplePattern);

	public bool Inp(object?[] tuplePattern, [MaybeNullWhen(false)] out object[] tuple);
	public bool Rdp(object?[] tuplePattern, [MaybeNullWhen(false)] out object[] tuple);
}

public interface IActionEvalLinda : ILinda {
	public void Eval(Action<IActionEvalLinda> func);
}

public interface ISpaceViewLinda : ILinda {
	public IEnumerable<object[]> ReadAll();
}