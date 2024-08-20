using Microsoft.AspNetCore.Mvc;

namespace LindaSharp.Server.Controllers;

[Route("api/scripts")]
[ApiController]
public class ScriptsController(IScriptEvalLinda linda) : ControllerBase {
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

		//TODO: Allow other languages
		var script = new IScriptEvalLinda.Script(
			IScriptEvalLinda.Script.Language.IronPython,
			await reader.ReadToEndAsync()
		);

		await linda.EvalScript(script);

		return Accepted();
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
		//TODO: Allow other languages
		var script = new IScriptEvalLinda.Script(
			IScriptEvalLinda.Script.Language.IronPython,
			await reader.ReadToEndAsync()
		);

		await linda.RegisterScript(key, script);

		return Created();
	}

	[HttpPost("eval/{key}")]
	public async Task<IActionResult> EvalInvoke(string key, [FromBody] object? parameter) {
		try {
			await linda.InvokeScript(key, parameter);
		} catch (KeyNotFoundException) {
			return NotFound();
		}

		return Accepted();
	}
}