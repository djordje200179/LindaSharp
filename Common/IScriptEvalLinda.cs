namespace LindaSharp;

public interface IScriptEvalLinda : ILinda {
	// TODO: Change to union enum
	public record struct ScriptExecutionStatus(ScriptExecutionStatus.ExecutionState State, Exception? Exception = null) {
		public enum ExecutionState {
			NotFound, Finished, Exception
		}
	}
	
	public Task RegisterScript(string key, string ironpythonCode);
	public Task<int> InvokeScript(string key, object? parameter = null);
	public Task<int> EvalScript(string ironpythonCode);

	public Task<ScriptExecutionStatus> GetScriptExecutionStatus(int id);
}

public static class IScriptEvalLindaExtensions {
	public static Task RegisterScriptFile(this IScriptEvalLinda linda, string key, string ironpythonFilePath) {
		return linda.RegisterScript(key, File.ReadAllText(ironpythonFilePath));
	}

	public static Task<int> EvalScriptFile(this IScriptEvalLinda linda, string ironpythonFilePath) {
		return linda.EvalScript(File.ReadAllText(ironpythonFilePath));
	}
}