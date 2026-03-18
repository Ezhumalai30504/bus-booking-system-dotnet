namespace BusBooking_API.Model
{
    public class BookingSeat
    {
        public int Id { get; set; }

        public int BookingId { get; set; }
        public Booking Booking { get; set; }

        public int SeatNumber { get; set; }
    }
}
