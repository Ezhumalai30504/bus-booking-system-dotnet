namespace BusBooking_MVC.Models.Booking
{
    public class BookingResponse
    {
        public int BookingId { get; set; }
        public string Message { get; set; } = "";
        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; } = "";
    }
}
