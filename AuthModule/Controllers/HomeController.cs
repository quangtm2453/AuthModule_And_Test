using Microsoft.AspNetCore.Mvc;

namespace AuthModule.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Auth");
            return View();
        }
        public IActionResult Admin()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
                return RedirectToAction("Login", "Auth");
            return View();
        }
    }
}
