namespace BusBooking_API.DTOs.BookingDTOs
{
    public class BookingResponseDto
    {
        public int BookingId { get; set; }
        public string Message { get; set; } = "";
        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; } = "";
    }
}
