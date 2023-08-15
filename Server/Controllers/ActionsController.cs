using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LindaSharp.Server.Controllers;

[Route("/")]
[ApiController]
public class ActionsController : ControllerBase {
	private readonly SharedLinda linda;

	public ActionsController(SharedLinda linda) {
		this.linda = linda;
	}

	[HttpPost("/out")]
	public IActionResult Out([FromBody] object[] tuple) {
		try {
			linda.Out(tuple);
		} catch (ObjectDisposedException) {
			return StatusCode((int)HttpStatusCode.InternalServerError);
		}

		return Created("/rd", tuple);
	}

	[HttpDelete("/in")]
	public IActionResult In([FromBody] object?[] tuplePattern) {
		try {
			var tuple = linda.In(tuplePattern);

			return Ok(tuple);
		} catch (ObjectDisposedException) {
			return StatusCode((int)HttpStatusCode.InternalServerError);
		}
	}

	[HttpGet("/rd")]
	public IActionResult Rd([FromBody] object?[] tuplePattern) {
		try {
			var tuple = linda.Rd(tuplePattern);

			return Ok(tuple);
		} catch (ObjectDisposedException) {
			return StatusCode((int)HttpStatusCode.InternalServerError);
		}
	}

	[HttpDelete("/inp")]
	public IActionResult Inp([FromBody] object?[] tuplePattern) {
		try {
			var status = linda.Inp(tuplePattern, out var tuple);

			return status ? Ok(tuple) : NotFound();
		} catch (ObjectDisposedException) {
			return StatusCode((int)HttpStatusCode.InternalServerError);
		}
	}

	[HttpGet("/rdp")]
	public IActionResult Rdp([FromBody] object?[] tuplePattern) {
		try {
			var status = linda.Rdp(tuplePattern, out var tuple);

			return status ? Ok(tuple) : NotFound();
		} catch (ObjectDisposedException) {
			return StatusCode((int)HttpStatusCode.InternalServerError);
		}
	}

	[HttpPost("/eval")]
	public IActionResult Eval() {
		var request = HttpContext.Request;

		StreamReader reader;
		if (request.ContentType == "text/ironpython")
			reader = new StreamReader(request.Body);
		else if (request.Form.Files.Count == 1) {
			var file = request.Form.Files[0];
			reader = new StreamReader(file.OpenReadStream());
		} else
			return BadRequest();

		var codeReadingTask = reader.ReadToEndAsync();
		codeReadingTask.Wait();
		var ironpythonCode = codeReadingTask.Result;

		linda.Eval(ironpythonCode);

		return Ok();
	}

	[HttpPut("/eval/{key}")]
	public IActionResult EvalRegister(string key) {
		var request = HttpContext.Request;

		StreamReader reader;
		if (request.ContentType == "text/ironpython")
			reader = new StreamReader(request.Body);
		else if (request.Form.Files.Count == 1) {
			var file = request.Form.Files[0];
			reader = new StreamReader(file.OpenReadStream());
		} else
			return BadRequest();

		var codeReadingTask = reader.ReadToEndAsync();
		codeReadingTask.Wait();
		var ironpythonCode = codeReadingTask.Result;

		linda.EvalRegister(key, ironpythonCode);

		return Ok();
	}

	[HttpPost("/eval/{key}")]
	public IActionResult EvalInvoke(string key, [FromBody] object? parameter) {
		try {
			linda.EvalInvoke(key, parameter);
		} catch (KeyNotFoundException) {
			return NotFound();
		}

		return Ok();
	}
}