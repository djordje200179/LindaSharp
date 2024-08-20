namespace LindaSharp;

public interface IActionEvalLinda : ILinda {
	public void Eval(Action<IActionEvalLinda> func);
}