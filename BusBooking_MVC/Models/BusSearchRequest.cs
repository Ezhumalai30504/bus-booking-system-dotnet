using System.ComponentModel.DataAnnotations;

namespace BusBooking_MVC.Models
{
    public class BusSearchRequest
    {
        [Required(ErrorMessage = "From City is required")]
        public string FromCity { get; set; }

        [Required(ErrorMessage = "To City is required")]
        public string ToCity { get; set; }

        [Required(ErrorMessage = "Travel Date is required")]
        [DataType(DataType.Date)]
        public DateTime TravelDate { get; set; }
    }
}
