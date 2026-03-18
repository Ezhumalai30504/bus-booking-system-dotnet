namespace BusBooking_API.DTOs.BookingDTOs
{
    public class BookingReadDto
    {
        public int BookingId { get; set; }
        public int BusId { get; set; }
        public string BusName { get; set; } = "";
        public DateTime TravelDate { get; set; }

        public string UserEmail { get; set; } = "";

        public List<int> SeatNumbers { get; set; } = new List<int>();

        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; } = "";

        public bool IsCancelled { get; set; }
    }
}
