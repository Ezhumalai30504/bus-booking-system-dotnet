using System.ComponentModel.DataAnnotations;

namespace BusBooking_MVC.Models
{
    public class Register
    {
        [Required]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@gmail\.com$", ErrorMessage = "Enter a valid Gmail address (example@gmail.com)")]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
