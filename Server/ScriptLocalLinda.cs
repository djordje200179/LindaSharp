using IronPython.Runtime.Operations;

namespace LindaSharp.Server;

public class ScriptLocalLinda(IActionEvalLinda linda) {
	private static object?[] ReformatTuple(object?[] tuple) {
		return tuple.Select(elem => elem switch {
			int intElem => intElem.ToBigInteger(),
			_ => elem
		}).ToArray();
	}

#pragma warning disable IDE1006 // Naming Styles
	public void @out(object[] tuple) => linda.Out(ReformatTuple(tuple)!);

	public object[] in_(object?[] pattern) => linda.In(ReformatTuple(pattern)).Result;
	public object[] rd(object[] pattern) => linda.Rd(ReformatTuple(pattern)).Result;

	public object[]? inp(object?[] pattern) => linda.Inp(ReformatTuple(pattern)).Result;
	public object[]? rdp(object?[] pattern) => linda.Rdp(ReformatTuple(pattern)).Result;

	public void eval(Action<ILinda> function) => linda.Eval(function);
#pragma warning restore IDE1006 // Naming Styles
}
