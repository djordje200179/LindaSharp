using Microsoft.AspNetCore.Mvc;
using static LindaSharp.IScriptEvalLinda;
using static LindaSharp.IScriptEvalLinda.ScriptExecutionStatus;

namespace LindaSharp.Server.Controllers;

[Route("api/health")]
[ApiController]
public class HealthController(IScriptEvalLinda linda) : ControllerBase {
	[HttpGet("ping")]
	public ActionResult Ping() {
		return Ok("pong");
	}

	[HttpGet("script/{id}")]
	public async Task<ActionResult> GetScriptExecutionStatus(int id) {
		var status = await linda.GetScriptExecutionStatus(id);
		return status switch {
			ScriptExecutionStatus(ExecutionState.NotFound, _) => NotFound(),
			ScriptExecutionStatus(ExecutionState.Finished, _) => Ok(),
			ScriptExecutionStatus(ExecutionState.Exception, var exception) when exception is not null =>
				BadRequest(exception),
			_ => throw new ArgumentException("invalid status")
		};
	}
}