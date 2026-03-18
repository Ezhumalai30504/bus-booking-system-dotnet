using BusBooking_API.Model;

namespace BusBooking_API.Repositary.Interfaces
{
    public interface IRouteRepositary
    {
        Task<IEnumerable<BusRoute>> GetAllAsync();
        Task<BusRoute?> GetByIdAsync(int id);
        Task AddAsync(BusRoute route);
        Task UpdateAsync(BusRoute route);
        Task DeleteAsync(int id);
    }
}
