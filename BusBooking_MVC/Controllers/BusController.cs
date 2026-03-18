using BusBooking_MVC.Models;
using BusBooking_MVC.Repositorys.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BusBooking_MVC.Controllers
{
    public class BusController : Controller
    {
        private readonly IBusRepository _repo;

        public BusController(IBusRepository repo)
        {
            _repo = repo;
        }

        // LIST BUSES (Admin Only)
        public async Task<IActionResult> Manage()
        {
            var token = HttpContext.Session.GetString("JWToken");
            var role = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            if (role != "Admin")
                return Unauthorized();

            var buses = await _repo.GetAllAsync(token);

            return View(buses);   // Model = List<BusRead>
        }

        // CREATE
        public IActionResult Create()
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (role != "Admin")
                return Unauthorized();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(BusCreate model)
        {
            var token = HttpContext.Session.GetString("JWToken");
            var role = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            if (role != "Admin")
                return Unauthorized();

            await _repo.AddAsync(model, token);

            return RedirectToAction("Manage");
        }

        // EDIT
        public async Task<IActionResult> Edit(int id)
        {
            var token = HttpContext.Session.GetString("JWToken");
            var role = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            if (role != "Admin")
                return Unauthorized();

            var bus = await _repo.GetByIdAsync(id, token);

            // Convert BusRead → BusUpdate
            var updateModel = new BusUpdate
            {
                Id = bus.Id,
                BusName = bus.BusName,
                TotalSeats = bus.TotalSeats,
                Price = bus.Price,
                RouteId = bus.RouteId
            };

            return View(updateModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(BusUpdate model)
        {
            var token = HttpContext.Session.GetString("JWToken");
            var role = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            if (role != "Admin")
                return Unauthorized();

            await _repo.UpdateAsync(model, token);

            return RedirectToAction("Manage");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var token = HttpContext.Session.GetString("JWToken");
            var role = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            if (role != "Admin")
                return Unauthorized();

            var ok = await _repo.DeleteAsync(id, token);

            if (!ok)
            {
                TempData["Error"] = "Delete failed. Check API / token / authorization.";
            }

            return RedirectToAction("Manage");
        }

        public async Task<IActionResult> ByRoute(int routeId, DateTime? travelDate)
        {
            if (travelDate == null)
            {
                TempData["Error"] = "Please select a travel date.";
                return RedirectToAction("ListRoute", "BusRoute");
            }

            if (travelDate.Value.Date < DateTime.Today)
            {
                TempData["Error"] = "Past dates are not allowed.";
                return RedirectToAction("ListRoute", "BusRoute");
            }

            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            var buses = await _repo.GetByRouteIdAndDateAsync(routeId, travelDate.Value, token);

            ViewBag.RouteId = routeId;
            ViewBag.TravelDate = travelDate.Value.ToString("yyyy-MM-dd");

            return View(buses);
        }
    }
}
