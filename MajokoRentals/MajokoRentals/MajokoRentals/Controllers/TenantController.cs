using Microsoft.AspNetCore.Mvc;

namespace MajokoRentals.Controllers
{
    public class TenantController : Controller
    {
        [HttpGet]
        public IActionResult Tenant()
        {
            ViewData["Title"] = "Tenant";
            return View();
        }
    }
}
