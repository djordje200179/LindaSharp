namespace LindaSharp.Server;

public class ScriptLocalLinda {
	private readonly StandardizedLocalLinda linda;

	public ScriptLocalLinda(IActionEvalLinda linda) {
		this.linda = new StandardizedLocalLinda(linda);
	}

#pragma warning disable IDE1006 // Naming Styles
	public void @out(object[] tuple) {
		linda.Out(tuple);
	}

	public object[] in_(object?[] tuple_pattern) {
		return linda.In(tuple_pattern);
	}

	public object[] rd(object[] tuple_pattern) {
		return linda.Rd(tuple_pattern);
	}

	public object[]? inp(object?[] tuple_pattern) {
		linda.Inp(tuple_pattern, out var tuple);
		return tuple;
	}

	public object[]? rdp(object?[] tuple_pattern) {
		linda.Rdp(tuple_pattern, out var tuple);
		return tuple;
	}

	public void eval(Action<ILinda> function) {
		linda.Eval(function);
	}
#pragma warning restore IDE1006 // Naming Styles
}
