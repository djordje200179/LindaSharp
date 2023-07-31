namespace LindaSharp;

public interface ILinda : IDisposable {
	public void Out(object[] tuple);

	public object[] In(object?[] tuplePattern);
	public bool Inp(object?[] tuplePattern, out object[]? tuple);

	public object[] Rd(object?[] tuplePattern);
	public bool Rdp(object?[] tuplePattern, out object[]? tuple);

	public void Eval(Action<ILinda> function);
	public void Eval(Action<ILinda, object> function, object parameter);
}