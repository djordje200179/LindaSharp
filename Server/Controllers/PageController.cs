using Microsoft.AspNetCore.Mvc;

namespace LindaSharp.Server.Controllers;

public class PageController : Controller {
	private readonly SharedLinda linda;

	public PageController(SharedLinda linda) {
		this.linda = linda;
	}

	[Route("/")]
	public IActionResult Index() {
		ViewBag.TupleSpace = linda.ReadAll();
		return View();
	}
}
