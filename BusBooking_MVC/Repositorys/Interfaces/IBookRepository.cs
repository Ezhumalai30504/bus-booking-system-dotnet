using BusBooking_MVC.Models.Booking;

namespace BusBooking_MVC.Repositorys.Interfaces
{
    public interface IBookRepository
    {
        Task<List<int>> GetAvailableSeatsAsync(int busId, DateTime date, string token);
        Task<BookingResponse?> BookAsync(BookingRequest dto, string token);
        // ✅ User cancel own booking (API: Booking/cancel/{id})
        Task<string> CancelByUserAsync(int bookingId, string token);

        // ✅ User booking list (API: Booking/my)
        Task<List<BookingReadVm>> MyBookingsAsync(string token);

        // ✅ Admin bookings list (API: Booking/admin/all)
        Task<List<BookingReadVm>> AdminAllBookingsAsync(string token);

        // ✅ Admin cancel any booking (API: Booking/admin/cancel/{id})
        Task<string> CancelByAdminAsync(int bookingId, string token);
    }
}
