using BusBooking_API.Model;

namespace BusBooking_API.Repositary.Interfaces
{
    public interface IBusRepositary
    {
        Task<IEnumerable<Bus>> GetAllAsync();

        Task<Bus> GetByIdAsync(int id);

        Task AddAsync(Bus bus);

        Task Update(Bus bus);

        Task Delete (Bus bus);

        Task<IEnumerable<Bus>> GetByRouteAndDateAsync(int routeId, DateTime travelDate);
    }
}
