using Microsoft.AspNetCore.Mvc;

namespace LindaSharp.Server.Controllers;

[Route("/")]
public class DashboardController(ISpaceViewLinda linda) : Controller {
	[HttpGet("")]
	public async Task<IActionResult> Index() {
		ViewBag.TupleSpace = await linda.QueryAll();
		return View();
	}
}
