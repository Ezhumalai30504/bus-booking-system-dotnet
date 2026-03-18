using Microsoft.AspNetCore.Mvc;

namespace BusBooking_MVC.Controllers
{
    public class AdminController : Controller
    {
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult AdminDashboard()
        {
            var token = HttpContext.Session.GetString("JWToken");
            var role = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(token) || role != "Admin")
            {
                return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.Email = HttpContext.Session.GetString("UserEmail");
            return View();
        }
    }
}
