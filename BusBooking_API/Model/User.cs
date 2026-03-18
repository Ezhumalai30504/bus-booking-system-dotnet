using System.ComponentModel.DataAnnotations;

namespace BusBooking_API.Model
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public string Role { get; set; } = "User";


        public ICollection<Booking> Bookings { get; set; }
    }
}
