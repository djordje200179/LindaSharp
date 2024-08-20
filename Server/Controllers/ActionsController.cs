using Microsoft.AspNetCore.Mvc;

namespace LindaSharp.Server.Controllers;

[Route("api/actions")]
[ApiController]
public class ActionsController(SharedLinda linda) : ControllerBase {
	[HttpPost("out")]
	public async Task<IActionResult> Out([FromBody] object[] tuple) {
		await linda.Put(tuple);

		return Created("/actions/rd", tuple);
	}

	[HttpDelete("in")]
	public async Task<IActionResult> In([FromBody] object?[] pattern) {
		return Ok(await linda.Get(pattern));
	}

	[HttpGet("rd")]
	public async Task<IActionResult> Rd([FromBody] object?[] pattern) {
		return Ok(await linda.Query(pattern));
	}

	[HttpDelete("inp")]
	public async Task<IActionResult> Inp([FromBody] object?[] pattern) {
		var tuple = await linda.TryGet(pattern);

		return tuple is not null ? Ok(tuple) : NotFound();
	}

	[HttpGet("rdp")]
	public async Task<IActionResult> Rdp([FromBody] object?[] pattern) {
		var tuple = await linda.TryQuery(pattern);

		return tuple is not null ? Ok(tuple) : NotFound();
	}
}