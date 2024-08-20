namespace LindaSharp.ScriptEngine;

public class ScriptLocalLinda(IActionEvalLinda linda) {
#pragma warning disable IDE1006 // Naming Styles
	public void put(params object[] tuple) => linda.Put(tuple);

	public object[] get(params object?[] pattern) => linda.Get(pattern).Result;
	public object[] query(params object[] pattern) => linda.Query(pattern).Result;

	public object[]? try_get(params object?[] pattern) => linda.TryGet(pattern).Result;
	public object[]? try_query(params object?[] pattern) => linda.TryQuery(pattern).Result;

	public void eval(Action<ILinda> function) => linda.Eval(function);
#pragma warning restore IDE1006 // Naming Styles
}
