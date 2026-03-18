namespace BusBooking_API.Model
{
    public class BusRoute
    {
        public int Id { get; set; }

        public string FromCity { get; set; }

        public string ToCity { get; set; }


        public ICollection<Bus> Buses { get; set; }
    }
}
