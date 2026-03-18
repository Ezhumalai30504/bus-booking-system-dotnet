using BusBooking_API.DTOs.BookingDTOs;
using BusBooking_API.Repositary.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BusBooking_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,User")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingRepositary _bookingRepository;

        public BookingController(IBookingRepositary bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        [HttpGet("available-seats/{busId}/{date}")]
        public async Task<IActionResult> GetAvailableSeats(int busId, DateTime date)
        {
            var seats = await _bookingRepository.GetAvailableSeats(busId, date);

            if (seats == null)
                return NotFound("Bus not found");

            return Ok(seats);
        }

        [HttpPost("book")]
        public async Task<IActionResult> BookSeat(BookingRequestDto dto)
        {
            var email = User.FindFirstValue(ClaimTypes.Name);

            var result = await _bookingRepository.Bookseat(dto, email);

            //  if booking failed, return BadRequest with JSON
            if (result.PaymentStatus != "Success")
                return BadRequest(result);

            //  return JSON with bookingId + amount
            return Ok(result);
        }

        //  User: Cancel ONLY own booking
        [Authorize(Roles = "User")]
        [HttpPost("cancel/{bookingId}")]
        public async Task<IActionResult> CancelByUser(int bookingId)
        {
            var email = User.FindFirstValue(ClaimTypes.Name);

            var result = await _bookingRepository.CancelBookingByUser(bookingId, email);

            if (result == "Booking Cancelled") return Ok(result);
            return BadRequest(result);
        }

        //  Admin: Cancel ANY booking
        [Authorize(Roles = "Admin")]
        [HttpPost("admin/cancel/{bookingId}")]
        public async Task<IActionResult> CancelByAdmin(int bookingId)
        {
            var result = await _bookingRepository.CancelBookingByAdmin(bookingId);

            if (result == "Booking Cancelled") return Ok(result);
            return BadRequest(result);
        }

        //  Admin Manage Bookings: View all bookings
        [Authorize(Roles = "Admin")]
        [HttpGet("admin/all")]
        public async Task<IActionResult> GetAllBookings()
        {
            var data = await _bookingRepository.GetAllBookings();
            return Ok(data);
        }

        //  Admin: Booking details
        [Authorize(Roles = "Admin")]
        [HttpGet("admin/{bookingId}")]
        public async Task<IActionResult> GetBookingById(int bookingId)
        {
            var data = await _bookingRepository.GetBookingById(bookingId);
            if (data == null) return NotFound("Booking not found");
            return Ok(data);
        }

        //  User: View my bookings
        [Authorize(Roles = "User")]
        [HttpGet("my")]
        public async Task<IActionResult> MyBookings()
        {
            var email = User.FindFirstValue(ClaimTypes.Name);
            var data = await _bookingRepository.GetBookingsByUser(email);
            return Ok(data);
        }
    }
}
