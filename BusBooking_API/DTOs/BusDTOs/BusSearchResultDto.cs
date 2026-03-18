namespace BusBooking_API.DTOs.BusDTOs
{
    public class BusSearchResultDto
    {
        public int Id { get; set; }
        public string BusName { get; set; } = string.Empty;
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
        public decimal Price { get; set; }

        public int RouteId { get; set; }
        public string FromCity { get; set; } = string.Empty;
        public string ToCity { get; set; } = string.Empty;
    }
}
