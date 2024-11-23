using LindaException = LindaSharp.Services.ScriptExecutionStatus.Types.Exception;

namespace LindaSharp.Client;

public class RemoteException(LindaException ex) : Exception {
	public override string Message => ex.Message;
	public override string StackTrace => ex.StackTrace;
	public override string Source => ex.Source;
}
