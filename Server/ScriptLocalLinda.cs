using IronPython.Runtime.Operations;
namespace LindaSharp.Server;

public class ScriptLocalLinda {
	private readonly IActionEvalLinda linda;

	public ScriptLocalLinda(IActionEvalLinda linda) {
		this.linda = linda;
	}

	private static object?[] ReformatTuple(object?[] tuple) {
		return tuple.Select(elem => elem switch {
			int intElem => intElem.ToBigInteger(),
			_ => elem
		}).ToArray();
	}

#pragma warning disable IDE1006 // Naming Styles
	public void @out(object[] tuple) {
		var reformattedTuple = ReformatTuple(tuple);

		linda.Out(reformattedTuple);
	}

	public object[] in_(object?[] tuple_pattern) {
		var reformattedTuplePattern = ReformatTuple(tuple_pattern);

		return linda.In(reformattedTuplePattern);
	}

	public object[] rd(object[] tuple_pattern) {
		var reformattedTuplePattern = ReformatTuple(tuple_pattern);

		return linda.Rd(reformattedTuplePattern);
	}

	public object[]? inp(object?[] tuple_pattern) {
		var reformattedTuplePattern = ReformatTuple(tuple_pattern);

		linda.Inp(reformattedTuplePattern, out var tuple);
		return tuple;
	}

	public object[]? rdp(object?[] tuple_pattern) {
		var reformattedTuplePattern = ReformatTuple(tuple_pattern);

		linda.Rdp(reformattedTuplePattern, out var tuple);
		return tuple;
	}

	public void eval(Action<ILinda> function) {
		linda.Eval(function);
	}
#pragma warning restore IDE1006 // Naming Styles
}
