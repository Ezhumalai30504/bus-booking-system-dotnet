namespace BusBooking_API.DTOs.BusDTOs
{
    public class BusReadDto
    {
        public int Id { get; set; }

        public string BusName { get; set; }

        public int TotalSeats { get; set; }

        public decimal Price { get; set; }

        public int RouteId { get; set; }
    }
}
