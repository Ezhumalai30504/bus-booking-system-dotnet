using System.Text.Json.Serialization;

namespace BusBooking_API.Model
{
    public class Bus
    {
        public int Id { get; set; }

        public string BusName { get; set; }

        public int TotalSeats { get; set; }

        public decimal Price { get; set; }

        public int RouteId { get; set; }


        public BusRoute? Route { get; set; }

        public ICollection<Booking>? Bookings { get; set; }
    }
}
