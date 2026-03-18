using BusBooking_MVC.Models;
using BusBooking_MVC.Repositorys.Implementations;
using BusBooking_MVC.Repositorys.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BusBooking_MVC.Controllers
{
    public class BusRouteController : Controller
    {
        private readonly IBusRouteRepository _repo;

        private readonly IBusSearchRepository _searchRepo;

        public BusRouteController(IBusRouteRepository repo , IBusSearchRepository searchRepo)
        {
            _repo = repo;
            _searchRepo = searchRepo;
        }

        // LIST ROUTES (Admin & User)
        public async Task<IActionResult> ListRoute(string? fromCity, string? toCity, DateTime? travelDate)
        {
            var token = HttpContext.Session.GetString("JWToken");
            var role = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            var routes = await _repo.GetAllAsync(token);

            // ✅ Prefill values for search form
            ViewBag.PrefillFromCity = fromCity;
            ViewBag.PrefillToCity = toCity;
            ViewBag.TravelDate = travelDate;

            ViewBag.SearchResults = null;

            if (role == "Admin")
                return View("Manage", routes);

            return View("ListRoute", routes);
        }

        [HttpGet]
        public IActionResult Search()
        {
            return View(new BusSearchRequest { TravelDate = DateTime.Today });
        }

        [HttpPost]
        public async Task<IActionResult> Search(BusSearchRequest model)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.FromCity = model.FromCity.Trim();
            model.ToCity = model.ToCity.Trim();

            // ✅ Past date not allowed
            if (model.TravelDate.Date < DateTime.Today)
            {
                ModelState.AddModelError("TravelDate", "Past dates are not allowed. Please select today or a future date.");
                ViewBag.SearchResults = null;  // ✅ buses not shown
                return View(model);
            }

            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            // ✅ Call MVC repository (your code)
            var results = await _searchRepo.SearchBusesAsync(
                model.FromCity, model.ToCity, model.TravelDate, token);

            ViewBag.SearchResults = results;
            return View(model);
        }

        // CREATE (Admin Only)
        public IActionResult Create()
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (role != "Admin")
                return Unauthorized();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(BusRoute model)
        {
            var token = HttpContext.Session.GetString("JWToken");
            var role = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            if (role != "Admin")
                return Unauthorized();

            await _repo.CreateAsync(model, token);

            return RedirectToAction("ListRoute");
        }

        // EDIT (Admin Only)
        public async Task<IActionResult> Edit(int id)
        {
            var token = HttpContext.Session.GetString("JWToken");
            var role = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            if (role != "Admin")
                return Unauthorized();

            var route = await _repo.GetByIdAsync(id, token);

            return View(route);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(BusRoute model)
        {
            var token = HttpContext.Session.GetString("JWToken");
            var role = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            if (role != "Admin")
                return Unauthorized();

            await _repo.UpdateAsync(model, token);

            return RedirectToAction("ListRoute");
        }

        // DELETE (Admin Only)
        public async Task<IActionResult> Delete(int id)
        {
            var token = HttpContext.Session.GetString("JWToken");
            var role = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            if (role != "Admin")
                return Unauthorized();

            await _repo.DeleteAsync(id, token);

            return RedirectToAction("ListRoute");
        }
    }
}
