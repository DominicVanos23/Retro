using Microsoft.AspNetCore.Mvc;

namespace MajokaRentals.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        public IActionResult Listings()
        {
            // You can pass a model or just return the view for now
            return View();
        }
    }
}
