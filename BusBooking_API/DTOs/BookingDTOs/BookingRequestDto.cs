namespace BusBooking_API.DTOs.BookingDTOs
{
    public class BookingRequestDto
    {
        public int BusId { get; set; }

        public DateTime TravelDate { get; set; }

        public List<int> SeatNumbers { get; set; }

    }
}
