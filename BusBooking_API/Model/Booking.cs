namespace BusBooking_API.Model
{
    public class Booking
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }


        public int BusId { get; set; }
        public Bus Bus { get; set; }

        public DateTime TravelDate { get; set; }

        public bool IsCancelled { get; set; } = false;

        public ICollection<BookingSeat> BookingSeats { get; set; }

        public Payment Payment { get; set; }
    }
}
