using Microsoft.AspNetCore.Mvc;

namespace LindaSharp.Server.Controllers;

public class DashboardController : Controller {
	private readonly SharedLinda linda;

	public DashboardController(SharedLinda linda) {
		this.linda = linda;
	}

	[Route("/")]
	public IActionResult Index() {
		ViewBag.TupleSpace = linda.ReadAll();
		return View();
	}
}
