using Microsoft.AspNetCore.Mvc;

namespace MajokoRentals.Controllers
{
    public class CommunicationController : Controller
    {
        [HttpGet]
        public IActionResult Communication()
        {
            ViewData["Title"] = "Communication";
            return View();
        }
    }
}
