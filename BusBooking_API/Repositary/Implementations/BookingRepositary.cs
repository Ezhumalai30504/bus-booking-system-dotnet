using BusBooking_API.DTOs.BookingDTOs;
using BusBooking_API.Model;
using BusBooking_API.Model.Database;
using BusBooking_API.Repositary.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusBooking_API.Repositary.Implementations
{
    public class BookingRepositary : IBookingRepositary
    {
        private readonly ApplicationDbContext _context;
        public BookingRepositary(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<int>> GetAvailableSeats(int busId , DateTime date)
        {
            var bus = await _context.Buses.Include(b => b.Bookings).ThenInclude(bs => bs.BookingSeats).FirstOrDefaultAsync(b  => b.Id == busId);

            if(bus == null)
            {
                return null;
            }

            var bookedSeats = bus.Bookings.Where(b => b.TravelDate.Date == date.Date && !b.IsCancelled)
                              .SelectMany(b => b.BookingSeats).Select(s => s.SeatNumber).ToList();

            var allSeats = Enumerable.Range(1, bus.TotalSeats);

            return allSeats.Except(bookedSeats);
        }

        
        public async Task<BookingResponseDto> Bookseat(BookingRequestDto dto, string userEmail)
        {
            var bus = await _context.Buses
                .Include(b => b.Bookings)
                .ThenInclude(bk => bk.BookingSeats)
                .FirstOrDefaultAsync(b => b.Id == dto.BusId);

            if (bus == null)
                return new BookingResponseDto { Message = "Bus not found", PaymentStatus = "Failed" };

            if (dto.SeatNumbers == null || !dto.SeatNumbers.Any())
                return new BookingResponseDto { Message = "No seats selected", PaymentStatus = "Failed" };

            var bookedSeats = bus.Bookings
                .Where(b => b.TravelDate.Date == dto.TravelDate.Date && !b.IsCancelled)
                .SelectMany(b => b.BookingSeats)
                .Select(s => s.SeatNumber)
                .ToList();

            if (dto.SeatNumbers.Any(seat => bookedSeats.Contains(seat)))
                return new BookingResponseDto { Message = "One or more seats already booked", PaymentStatus = "Failed" };

            if (dto.SeatNumbers.Any(seat => seat < 1 || seat > bus.TotalSeats))
                return new BookingResponseDto { Message = "Invalid seat number selected", PaymentStatus = "Failed" };

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
                return new BookingResponseDto { Message = "User not found", PaymentStatus = "Failed" };

            var totalAmount = dto.SeatNumbers.Count * bus.Price;

            var booking = new Booking
            {
                BusId = dto.BusId,
                UserId = user.Id,
                TravelDate = dto.TravelDate,
                BookingSeats = dto.SeatNumbers.Select(seat => new BookingSeat
                {
                    SeatNumber = seat
                }).ToList(),
                Payment = new Payment
                {
                    Amount = totalAmount,
                    Status = "Success"
                }
            };

            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();

            
            return new BookingResponseDto
            {
                BookingId = booking.Id,
                Message = "Booking Confirmed",
                Amount = totalAmount,
                PaymentStatus = "Success"
            };
        }


        public async Task<List<BookingReadDto>> GetAllBookings()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Bus)
                .Include(b => b.User)
                .Include(b => b.BookingSeats)
                .Include(b => b.Payment)
                .OrderByDescending(b => b.Id)
                .ToListAsync();

            return bookings.Select(MapToReadDto).ToList();
        }

        public async Task<BookingReadDto?> GetBookingById(int bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Bus)
                .Include(b => b.User)
                .Include(b => b.BookingSeats)
                .Include(b => b.Payment)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null) return null;

            return MapToReadDto(booking);
        }

        public async Task<List<BookingReadDto>> GetBookingsByUser(string userEmail)
        {
            var bookings = await _context.Bookings
                .Include(b => b.Bus)
                .Include(b => b.User)
                .Include(b => b.BookingSeats)
                .Include(b => b.Payment)
                .Where(b => b.User.Email == userEmail)
                .OrderByDescending(b => b.Id)
                .ToListAsync();

            return bookings.Select(MapToReadDto).ToList();
        }

        // ✅ User can cancel only his booking
        public async Task<string> CancelBookingByUser(int bookingId, string userEmail)
        {
            var booking = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Payment)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null) return "Booking not found";

            if (booking.User.Email != userEmail)
                return "You can cancel only your booking";

            if (booking.IsCancelled)
                return "Booking already cancelled";

            booking.IsCancelled = true;

            if (booking.Payment != null)
                booking.Payment.Status = "Refund Pending"; // optional

            await _context.SaveChangesAsync();

            return "Booking Cancelled";
        }

        // ✅ Admin can cancel any booking
        public async Task<string> CancelBookingByAdmin(int bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Payment)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null) return "Booking not found";

            if (booking.IsCancelled)
                return "Booking already cancelled";

            booking.IsCancelled = true;

            if (booking.Payment != null)
                booking.Payment.Status = "Refund Pending"; // optional

            await _context.SaveChangesAsync();

            return "Booking Cancelled";
        }

        // ✅ Helper mapper
        private BookingReadDto MapToReadDto(Booking booking)
        {
            return new BookingReadDto
            {
                BookingId = booking.Id,
                BusId = booking.BusId,
                BusName = booking.Bus?.BusName ?? "",
                TravelDate = booking.TravelDate,
                UserEmail = booking.User?.Email ?? "",

                SeatNumbers = booking.BookingSeats?
                    .Select(s => s.SeatNumber)
                    .ToList() ?? new List<int>(),

                Amount = booking.Payment?.Amount ?? 0,
                PaymentStatus = booking.Payment?.Status ?? "",
                IsCancelled = booking.IsCancelled
            };
        }
    }
}
    