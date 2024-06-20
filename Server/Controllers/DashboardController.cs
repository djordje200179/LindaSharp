using Microsoft.AspNetCore.Mvc;

namespace LindaSharp.Server.Controllers;

[Route("/")]
public class DashboardController(SharedLinda linda) : Controller {
	[HttpGet("")]
	public IActionResult Index() {
		ViewBag.TupleSpace = linda.ReadAll();
		return View();
	}
}
