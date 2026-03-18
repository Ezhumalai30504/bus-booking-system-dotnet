namespace BusBooking_MVC.Models
{
    public class BusSearchResult
    {
        public int Id { get; set; }
        public string BusName { get; set; }
        public string FromCity { get; set; }
        public string ToCity { get; set; }
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
        public decimal Price { get; set; }
    }
}
