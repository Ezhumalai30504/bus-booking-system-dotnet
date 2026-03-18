namespace BusBooking_MVC.Models.Booking
{
    public class BookingCreateVM
    {
        public int BusId { get; set; }
        public string? BusName { get; set; }

        public DateTime TravelDate { get; set; }
        public int TotalSeats { get; set; }

        // Optional (if you have price from search)
        public decimal Price { get; set; }

        // Seat grid
        public List<SeatVM> Seats { get; set; } = new();

        // Posted selected seats
        public List<int> SelectedSeats { get; set; } = new();
    }
}
