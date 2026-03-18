using BusBooking_API.DTOs.BookingDTOs;

namespace BusBooking_API.Repositary.Interfaces
{
    public interface IBookingRepositary
    {
        Task<IEnumerable<int>> GetAvailableSeats(int busId, DateTime date);

        // ✅ changed return type from string to BookingResponseDto
        Task<BookingResponseDto> Bookseat(BookingRequestDto dto, string userEmail);

        // ✅ User cancels his own booking
        Task<string> CancelBookingByUser(int bookingId, string userEmail);

        // ✅ Admin cancels any booking
        Task<string> CancelBookingByAdmin(int bookingId);

        // ✅ Admin Manage Bookings
        Task<List<BookingReadDto>> GetAllBookings();

        Task<BookingReadDto?> GetBookingById(int bookingId);

        // ✅ User view his own bookings
        Task<List<BookingReadDto>> GetBookingsByUser(string userEmail);
    }
}
