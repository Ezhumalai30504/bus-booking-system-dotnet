using BusBooking_MVC.Models.Booking;
using BusBooking_MVC.Repositorys.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BusBooking_MVC.Controllers
{
    public class BookingController : Controller
    {
        private readonly IBookRepository _bookingRepo;
        private readonly IBusRepository _busRepo; 

        public BookingController(IBookRepository bookingRepo, IBusRepository busRepo)
        {
            _bookingRepo = bookingRepo;
            _busRepo = busRepo;
        }

        
        [HttpGet]
        public async Task<IActionResult> Create(int busId, DateTime travelDate)
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            
            var bus = await _busRepo.GetByIdAsync(busId, token);

            if (bus == null)
                return RedirectToAction("Search", "BusRoute");

            var available = await _bookingRepo.GetAvailableSeatsAsync(busId, travelDate, token);

            var vm = new BookingCreateVM
            {
                BusId = bus.Id,
                BusName = bus.BusName,
                TravelDate = travelDate,
                TotalSeats = bus.TotalSeats,
                Price = bus.Price
            };

            for (int i = 1; i <= bus.TotalSeats; i++)
            {
                vm.Seats.Add(new SeatVM
                {
                    Number = i,
                    IsAvailable = available.Contains(i)
                });
            }

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(BookingCreateVM vm)
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            if (vm.SelectedSeats == null || !vm.SelectedSeats.Any())
            {
                ModelState.AddModelError("", "Please select at least one seat.");
                // rebuild seats UI
                var available = await _bookingRepo.GetAvailableSeatsAsync(vm.BusId, vm.TravelDate, token);
                vm.Seats = new List<SeatVM>();
                for (int i = 1; i <= vm.TotalSeats; i++)
                {
                    vm.Seats.Add(new SeatVM { Number = i, IsAvailable = available.Contains(i) });
                }
                return View(vm);
            }

            var dto = new BookingRequest
            {
                BusId = vm.BusId,
                TravelDate = vm.TravelDate,
                SeatNumbers = vm.SelectedSeats
            };

            var res = await _bookingRepo.BookAsync(dto, token);

            // If API still returns only "Booking Confirmed" string, res.BookingId might be 0.
            if (res == null || string.IsNullOrEmpty(res.Message) || res.PaymentStatus == "Failed")
            {
                TempData["Error"] = res?.Message ?? "Booking failed.";
                return RedirectToAction("Create", new { busId = vm.BusId, travelDate = vm.TravelDate });
            }

       
            HttpContext.Session.SetString("BookingId", res.BookingId.ToString());
            HttpContext.Session.SetString("Message", res.Message ?? "");
            HttpContext.Session.SetString("Amount", res.Amount.ToString());
            HttpContext.Session.SetString("PaymentStatus", res.PaymentStatus ?? "Success");

            return RedirectToAction("Success");
        }

        [HttpGet]
        public IActionResult Success(int? bookingId)
        {
            ViewBag.BookingId = HttpContext.Session.GetString("BookingId");
            ViewBag.Message = HttpContext.Session.GetString("Message");
            ViewBag.Amount = HttpContext.Session.GetString("Amount");
            ViewBag.PaymentStatus = HttpContext.Session.GetString("PaymentStatus");

          
            if (string.IsNullOrEmpty(ViewBag.Message))
                return RedirectToAction("Index", "Dashboard");

            return View();
        }

        //  USER Cancel own booking
        [HttpPost]
        public async Task<IActionResult> Cancel(int bookingId)
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            var msg = await _bookingRepo.CancelByUserAsync(bookingId, token);

            TempData["CancelMessage"] = msg;
            return RedirectToAction("CancelResult");
        }

        [HttpGet]
        public IActionResult CancelResult()
        {
            return View();
        }

        
        [HttpGet]
        public IActionResult CancelForm(int? bookingId)
        {
            ViewBag.BookingId = bookingId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CancelForm(int bookingId)
        {
            return await Cancel(bookingId);
        }

        //  USER: My Bookings
        [HttpGet]
        public async Task<IActionResult> MyBookings()
        {

            var token = HttpContext.Session.GetString("JWToken");
            var role = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Account");

            if (role != "User")
                return Unauthorized();

            var list = await _bookingRepo.MyBookingsAsync(token);
            return View(list);
        }

        //  ADMIN: Manage Bookings
        [HttpGet]
        public async Task<IActionResult> AdminManage()
        {
            var token = HttpContext.Session.GetString("JWToken");
            var role = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("AdminLogin", "Account");

            if (role != "Admin")
                return Unauthorized();

            var list = await _bookingRepo.AdminAllBookingsAsync(token);
            return View(list);
        }

        [HttpPost]
        public async Task<IActionResult> AdminCancel(int bookingId)
        {
            var token = HttpContext.Session.GetString("JWToken");
            var role = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("AdminLogin", "Account");

            if (role != "Admin")
                return Unauthorized();

            var msg = await _bookingRepo.CancelByAdminAsync(bookingId, token);
            TempData["Message"] = msg;

            return RedirectToAction("AdminManage");
        }
    }
}
