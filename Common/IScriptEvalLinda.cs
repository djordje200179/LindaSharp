namespace LindaSharp;

public interface IScriptEvalLinda : ILinda {
	public record struct Script(Script.Language Type, string Code) {
		public enum Language {
			IronPython, CSharp
		}
	}

	// TODO: Change to union enum
	public record struct ScriptExecutionStatus(ScriptExecutionStatus.ExecutionState State, Exception? Exception = null) {
		public enum ExecutionState {
			NotFound, Finished, Exception
		}
	}
	
	public Task RegisterScript(string key, Script script);
	public Task<int> InvokeScript(string key, object? parameter = null);
	public Task<int> EvalScript(Script script);

	public Task<ScriptExecutionStatus> GetScriptExecutionStatus(int id);
}

public static class IScriptEvalLindaExtensions {
	public static Task RegisterScriptFile(this IScriptEvalLinda linda, string key, string filePath) {
		var type = Path.GetExtension(filePath) switch {
			".py" => IScriptEvalLinda.Script.Language.IronPython,
			".cs" => IScriptEvalLinda.Script.Language.CSharp,
			_ => throw new ArgumentException("Unsupported file")
		};

		return linda.RegisterScript(key, new IScriptEvalLinda.Script(type, File.ReadAllText(filePath)));
	}

	public static Task<int> EvalScriptFile(this IScriptEvalLinda linda, string filePath) {
		var type = Path.GetExtension(filePath) switch {
			".py" => IScriptEvalLinda.Script.Language.IronPython,
			".cs" => IScriptEvalLinda.Script.Language.CSharp,
			_ => throw new ArgumentException("Unsupported file")
		};

		return linda.EvalScript(new IScriptEvalLinda.Script(type, File.ReadAllText(filePath)));
	}
}