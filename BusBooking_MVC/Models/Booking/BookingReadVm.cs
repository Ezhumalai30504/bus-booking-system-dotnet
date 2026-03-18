namespace BusBooking_MVC.Models.Booking
{
    public class BookingReadVm
    {
        public int BookingId { get; set; }
        public int BusId { get; set; }
        public string BusName { get; set; } = "";
        public DateTime TravelDate { get; set; }
        public string UserEmail { get; set; } = "";
        public List<int> SeatNumbers { get; set; } = new();
        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; } = "";
        public bool IsCancelled { get; set; }
    }
}
