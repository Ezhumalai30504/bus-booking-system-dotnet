using System.ComponentModel.DataAnnotations;

namespace BusBooking_MVC.Models
{
    public class VerifyOtp
    {
        [Required]
        public string Email { get; set; }

        [Required(ErrorMessage = "OTP is required")]
        public string OtpCode { get; set; }
    }
}
