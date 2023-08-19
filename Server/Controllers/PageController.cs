using Microsoft.AspNetCore.Mvc;

namespace LindaSharp.Server.Controllers;

public class PageController : Controller {
	[Route("/")]
	public IActionResult Index() {
		return View();
	}
}
