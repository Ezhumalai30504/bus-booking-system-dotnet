namespace BusBooking_MVC.Models.Booking
{
    public class BookingRequest
    {
        public int BusId { get; set; }
        public DateTime TravelDate { get; set; }
        public List<int> SeatNumbers { get; set; } = new();
    }
}
