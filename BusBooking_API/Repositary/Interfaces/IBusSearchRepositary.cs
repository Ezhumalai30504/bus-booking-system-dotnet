using BusBooking_API.DTOs.BusDTOs;

namespace BusBooking_API.Repositary.Interfaces
{
    public interface IBusSearchRepositary
    {
        Task<List<BusSearchResultDto>> SearchBusesAsync(string from, string to, DateTime travelDate);
    }
}
