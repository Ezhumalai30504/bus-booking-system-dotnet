using BusBooking_API.Model;
using BusBooking_API.Model.Database;
using BusBooking_API.Repositary.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusBooking_API.Repositary.Implementations
{
    public class BusRepositary : IBusRepositary
    {
        private readonly ApplicationDbContext _context;

        public BusRepositary(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Bus>> GetAllAsync()
        {
            return await _context.Buses.ToListAsync();
        }

        public async Task<Bus> GetByIdAsync(int id)
        {
            return await _context.Buses.FindAsync(id);
        }

        public async Task AddAsync(Bus bus)
        {
             _context.Buses.AddAsync(bus);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Bus bus)
        {
            _context.Buses.Update(bus);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Bus bus)
        {
            _context.Buses.Remove(bus);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Bus>> GetByRouteAndDateAsync(int routeId, DateTime travelDate)
        {

            if (travelDate.Date < DateTime.Today)
            {
                return new List<Bus>();
            }
            return await _context.Buses
                                 .Include(b => b.Route)
                                 .Where(b => b.RouteId == routeId)
                                 .ToListAsync();
        }
    }
}
