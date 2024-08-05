using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LindaSharp.Server.Controllers;

[Route("api/actions")]
[ApiController]
public class ActionsController(SharedLinda linda) : ControllerBase {
	[HttpPost("out")]
	public async Task<IActionResult> Out([FromBody] object[] tuple) {
		try {
			await linda.Out(tuple);
		} catch (Exception) {
			return StatusCode((int)HttpStatusCode.InternalServerError);
		}

		return Created("/actions/rd", tuple);
	}

	[HttpDelete("in")]
	public async Task<IActionResult> In([FromBody] object?[] pattern) {
		try {
			var tuple = await linda.In(pattern);

			return Ok(tuple);
		} catch (Exception) {
			return StatusCode((int)HttpStatusCode.InternalServerError);
		}
	}

	[HttpGet("rd")]
	public async Task<IActionResult> Rd([FromBody] object?[] pattern) {
		try {
			var tuple = await linda.Rd(pattern);

			return Ok(tuple);
		} catch (Exception) {
			return StatusCode((int)HttpStatusCode.InternalServerError);
		}
	}

	[HttpDelete("inp")]
	public async Task<IActionResult> Inp([FromBody] object?[] pattern) {
		try {
			var tuple = await linda.Inp(pattern);

			return tuple != null ? Ok(tuple) : NotFound();
		} catch (Exception) {
			return StatusCode((int)HttpStatusCode.InternalServerError);
		}
	}

	[HttpGet("rdp")]
	public async Task<IActionResult> Rdp([FromBody] object?[] pattern) {
		try {
			var tuple = await linda.Rdp(pattern);

			return tuple != null ? Ok(tuple) : NotFound();
		} catch (Exception) {
			return StatusCode((int)HttpStatusCode.InternalServerError);
		}
	}

	[HttpPost("eval")]
	public async Task<IActionResult> Eval() {
		var request = HttpContext.Request;

		StreamReader reader;
		if (request.ContentType == "text/ironpython") {
			reader = new StreamReader(request.Body);
		} else if (request.Form.Files.Count == 1) {
			var file = request.Form.Files[0];
			reader = new StreamReader(file.OpenReadStream());
		} else {
			return BadRequest();
		}

		await linda.Eval(await reader.ReadToEndAsync());

		return Ok();
	}

	[HttpPut("eval/{key}")]
	public async Task<IActionResult> EvalRegister(string key) {
		var request = HttpContext.Request;

		StreamReader reader;
		if (request.ContentType == "text/ironpython") {
			reader = new StreamReader(request.Body);
		} else if (request.Form.Files.Count == 1) {
			var file = request.Form.Files[0];
			reader = new StreamReader(file.OpenReadStream());
		} else {
			return BadRequest();
		}

		await linda.EvalRegister(key, await reader.ReadToEndAsync());

		return Ok();
	}

	[HttpPost("eval/{key}")]
	public async Task<IActionResult> EvalInvoke(string key, [FromBody] object? parameter) {
		try {
			await linda.EvalInvoke(key, parameter);
		} catch (KeyNotFoundException) {
			return NotFound();
		}

		return Ok();
	}
}