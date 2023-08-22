using Microsoft.AspNetCore.Mvc;

namespace LindaSharp.Server.Controllers;

[Route("/health")]
[ApiController]
public class HealthController : ControllerBase {
	[HttpGet("/ping")]
	public ActionResult Ping() {
		return Ok("pong");
	}
}