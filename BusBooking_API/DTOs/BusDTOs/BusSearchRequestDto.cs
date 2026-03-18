using System.ComponentModel.DataAnnotations;

namespace BusBooking_API.DTOs.BusDTOs
{
    public class BusSearchRequestDto
    {
        [Required]
        public string FromCity { get; set; } = string.Empty;

        [Required]
        public string ToCity { get; set; } = string.Empty;

        [Required]
        public DateTime TravelDate { get; set; }
    }
}
