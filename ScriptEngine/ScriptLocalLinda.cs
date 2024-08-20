using IronPython.Runtime.Operations;

namespace LindaSharp.ScriptEngine;

public class ScriptLocalLinda(IActionEvalLinda linda) {
	private static object?[] ReformatTuple(object?[] tuple) {
		return tuple.Select(elem => elem switch {
			int intElem => intElem.ToBigInteger(),
			_ => elem
		}).ToArray();
	}

#pragma warning disable IDE1006 // Naming Styles
	public void put(params object[] tuple) => linda.Put(ReformatTuple(tuple)!);

	public object[] get(params object?[] pattern) => linda.Get(ReformatTuple(pattern)).Result;
	public object[] query(params object[] pattern) => linda.Query(ReformatTuple(pattern)).Result;

	public object[]? try_get(params object?[] pattern) => linda.TryGet(ReformatTuple(pattern)).Result;
	public object[]? try_query(params object?[] pattern) => linda.TryQuery(ReformatTuple(pattern)).Result;

	public void eval(Action<ILinda> function) => linda.Eval(function);
#pragma warning restore IDE1006 // Naming Styles
}
