using BusBooking_API.Model;
using BusBooking_API.Model.Database;
using BusBooking_API.Repositary.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusBooking_API.Repositary.Implementations
{
    public class RouteRepositary : IRouteRepositary
    {
        private readonly ApplicationDbContext _context;

        public RouteRepositary(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BusRoute>> GetAllAsync()
        {
          return await _context.BusRoutes.ToListAsync();
        }
            

        public async Task<BusRoute?> GetByIdAsync(int id)
        {
            return await _context.BusRoutes.FindAsync(id);
        }

        public async Task AddAsync(BusRoute route)
        {
            await _context.BusRoutes.AddAsync(route);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(BusRoute route)
        {
            _context.BusRoutes.Update(route);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var route = await _context.BusRoutes.FindAsync(id);
            if (route != null)
            {
                _context.BusRoutes.Remove(route);
                await _context.SaveChangesAsync();
            }
        }
    }
}
