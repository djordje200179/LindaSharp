using IronPython.Runtime.Operations;

namespace LindaSharp.Server;

public class ScriptLocalLinda(IActionEvalLinda linda) {
	private static object?[] ReformatTuple(object?[] tuple) {
		return tuple.Select(elem => elem switch {
			int intElem => intElem.ToBigInteger(),
			_ => elem
		}).ToArray();
	}

	// TODO: change to async/await
	private static T RunTask<T>(Task<T> task) {
		task.Wait();
		return task.Result;
	}

#pragma warning disable IDE1006 // Naming Styles
	public void @out(object[] tuple) => linda.Out(ReformatTuple(tuple)!);

	public object[] in_(object?[] pattern) => RunTask(linda.In(ReformatTuple(pattern)));
	public object[] rd(object[] pattern) => RunTask(linda.Rd(ReformatTuple(pattern)));

	public object[]? inp(object?[] pattern) => RunTask(linda.Inp(ReformatTuple(pattern)));
	public object[]? rdp(object?[] pattern) => RunTask(linda.Rdp(ReformatTuple(pattern)));

	public void eval(Action<ILinda> function) => linda.Eval(function);
#pragma warning restore IDE1006 // Naming Styles
}
