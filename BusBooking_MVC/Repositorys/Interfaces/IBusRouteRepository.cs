using BusBooking_MVC.Models;

namespace BusBooking_MVC.Repositorys.Interfaces
{
    public interface IBusRouteRepository
    {
        Task<List<BusRoute>> GetAllAsync(string token);
        Task<bool> CreateAsync(BusRoute model, string token);
        Task<bool> UpdateAsync(BusRoute model, string token);
        Task<bool> DeleteAsync(int id, string token);
        Task<BusRoute> GetByIdAsync(int id, string token);
    }
}
