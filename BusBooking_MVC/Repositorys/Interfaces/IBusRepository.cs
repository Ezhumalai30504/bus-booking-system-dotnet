using BusBooking_MVC.Models;

namespace BusBooking_MVC.Repositorys.Interfaces
{
    public interface IBusRepository
    {
        Task<List<BusRead>> GetAllAsync(string token);
        Task<BusRead> GetByIdAsync(int id, string token);
        Task<bool> AddAsync(BusCreate dto, string token);
        Task<bool> UpdateAsync(BusUpdate dto, string token);
        Task<bool> DeleteAsync(int id, string token);
        Task<List<BusRead>> SearchAsync(string from, string to, DateTime date, string token);
        Task<List<BusRead>> GetByRouteIdAndDateAsync(int routeId, DateTime travelDate, string token);
    }
}
