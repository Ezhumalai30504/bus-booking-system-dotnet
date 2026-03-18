using Microsoft.AspNetCore.Mvc;

namespace BusBooking_MVC.Controllers
{
    public class DashboardController : Controller
    {
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Index()
        {
            var email = HttpContext.Session.GetString("UserEmail");
            var role = HttpContext.Session.GetString("UserRole");

            // Admin should not go to normal dashboard
            if (!string.IsNullOrEmpty(email) && role == "Admin")
            {
                return RedirectToAction("AdminDashboard", "Admin");
            }

            ViewBag.Email = email;
            ViewBag.Role = role;
            ViewData["Title"] = "Dashboard";

            return View();
        }
    }
}
