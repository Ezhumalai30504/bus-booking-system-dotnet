namespace BusBooking_MVC.Models
{
    public class BusCreate
    {
        public string BusName { get; set; }
        public int TotalSeats { get; set; }
        public decimal Price { get; set; }
        public int RouteId { get; set; }
    }
}
