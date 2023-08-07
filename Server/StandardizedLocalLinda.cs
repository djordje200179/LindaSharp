using IronPython.Runtime.Operations;
using System.Diagnostics.CodeAnalysis;

namespace LindaSharp.Server;

public class StandardizedLocalLinda : IActionEvalLinda {
	private readonly IActionEvalLinda linda;

	public StandardizedLocalLinda(IActionEvalLinda linda) {
		this.linda = linda;
	}

	private static object?[] ReformatTuple(object?[] tuple) {
		return tuple.Select(elem => elem switch {
			int intElem => intElem.ToBigInteger(),
			_ => elem
		}).ToArray();
	}

	public void Out(object[] tuple) {
		var reformattedTuple = ReformatTuple(tuple);

		linda.Out(reformattedTuple);
	}

	public object[] In(object?[] tuplePattern) {
		var reformattedTuplePattern = ReformatTuple(tuplePattern);

		return linda.In(reformattedTuplePattern);
	}

	public bool Inp(object?[] tuplePattern, [MaybeNullWhen(false)] out object[] tuple) {
		var reformattedTuplePattern = ReformatTuple(tuplePattern);

		return linda.Inp(reformattedTuplePattern, out tuple);
	}

	public object[] Rd(object?[] tuplePattern) {
		var reformattedTuplePattern = ReformatTuple(tuplePattern);

		return linda.Rd(reformattedTuplePattern);
	}

	public bool Rdp(object?[] tuplePattern, [MaybeNullWhen(false)] out object[] tuple) {
		var reformattedTuplePattern = ReformatTuple(tuplePattern);

		return linda.Rdp(reformattedTuplePattern, out tuple);
	}

	public void Eval(Action<IActionEvalLinda> function) {
		linda.Eval(function);
	}

	public void Dispose() {
		GC.SuppressFinalize(this);
		linda.Dispose();
	}
}
