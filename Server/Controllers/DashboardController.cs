using Microsoft.AspNetCore.Mvc;

namespace LindaSharp.Server.Controllers;

[Route("/")]
public class DashboardController : Controller {
	private readonly SharedLinda linda;

	public DashboardController(SharedLinda linda) {
		this.linda = linda;
	}

	[HttpGet("")]
	public IActionResult Index() {
		ViewBag.TupleSpace = linda.ReadAll();
		return View();
	}
}
