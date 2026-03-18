using BusBooking_MVC.Models;
using BusBooking_MVC.Repositorys.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BusBooking_MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthRepository _authRepo;

        public AccountController(IAuthRepository authRepo)
        {
            _authRepo = authRepo;
        }

        // GET: Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: Register
        [HttpPost]
        public async Task<IActionResult> Register(Register model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _authRepo.RegisterAsync(model);
            if (result)
            {
                TempData["Email"] = model.Email;
                return RedirectToAction("VerifyOtp");
            }

            ModelState.AddModelError("", "Registration failed.");
            return View(model);
        }

        // GET: VerifyOtp
        public IActionResult VerifyOtp()
        {
            var email = TempData["Email"]?.ToString();
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Register");
            }                

            
            TempData.Keep("Email");

            return View(new VerifyOtp { Email = email });
        }

        // POST: VerifyOtp
        [HttpPost]
        public async Task<IActionResult> VerifyOtp(VerifyOtp model)
        {
            
            if (string.IsNullOrEmpty(model.Email))
            {
                model.Email = TempData["Email"]?.ToString();
                TempData.Keep("Email");
            }

            if (!ModelState.IsValid)
                return View(model);

            var result = await _authRepo.VerifyOtpAsync(model);

            if (result)
            {
                TempData["Message"] = "Registration successful. Please login.";
                return RedirectToAction("Login");
            }

            ModelState.AddModelError(nameof(model.OtpCode), "Invalid OTP.");
            return View(model); 
        }

        // GET: User Login
        public IActionResult Login()
        {
            var token = HttpContext.Session.GetString("JWToken");
            var role = HttpContext.Session.GetString("UserRole");

            if (!string.IsNullOrEmpty(token))
            {
                if (role == "Admin")
                    return RedirectToAction("AdminDashboard", "Admin");

                return RedirectToAction("Index", "Dashboard");
            }

            return View();
        }

        // POST: User Login
        [HttpPost]
        public async Task<IActionResult> Login(Login model)
        {
            if (!ModelState.IsValid) return View(model);

            var loginResponse = await _authRepo.LoginAsync(model);
            if (loginResponse == null || loginResponse.Role != "User")
            {
                ModelState.AddModelError("", "Invalid credentials or not a User.");
                return View(model);
            }

            HttpContext.Session.SetString("JWToken", loginResponse.Token);
            HttpContext.Session.SetString("UserEmail", loginResponse.Email);
            HttpContext.Session.SetString("UserRole", loginResponse.Role);

            return RedirectToAction("Index", "Dashboard");
        }

        // GET: Admin Login
        public IActionResult AdminLogin()
        {
            var token = HttpContext.Session.GetString("JWToken");
            var role = HttpContext.Session.GetString("UserRole");

            if (!string.IsNullOrEmpty(token))
            {
                if (role == "Admin")
                    return RedirectToAction("AdminDashboard", "Admin");

                return RedirectToAction("Index", "Dashboard");
            }

            return View();
        }

        // POST: Admin Login (Only Admin)
        [HttpPost]
        public async Task<IActionResult> AdminLogin(Login model)
        {
            if (!ModelState.IsValid) return View(model);

            var loginResponse = await _authRepo.LoginAsync(model);

            if (loginResponse == null || loginResponse.Role != "Admin")
            {
                ModelState.AddModelError("", "Invalid credentials or not an Admin.");
                return View(model);
            }

            HttpContext.Session.SetString("JWToken", loginResponse.Token);
            HttpContext.Session.SetString("UserEmail", loginResponse.Email);
            HttpContext.Session.SetString("UserRole", loginResponse.Role);

            return RedirectToAction("AdminDashboard", "Admin");
        }

        // Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Dashboard");
        }

        public IActionResult AdminLogout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Dashboard");
        }
    }
}
