using BusBooking_API.DTOs.BusDTOs;
using BusBooking_API.Model.Database;
using BusBooking_API.Repositary.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace BusBooking_API.Repositary.Implementations
{
    public class BusSearchRepositary : IBusSearchRepositary
    {
        private readonly ApplicationDbContext _context;

        public BusSearchRepositary(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<BusSearchResultDto>> SearchBusesAsync(string from, string to, DateTime travelDate)
        {
            if (travelDate.Date < DateTime.Today)
            {
                return new List<BusSearchResultDto>();
            }

            var fromCity = from.Trim().ToLower();
            var toCity = to.Trim().ToLower();

            var buses = await _context.Buses.Include(b => b.Route).Include(b => b.Bookings)
                       .ThenInclude(bk => bk.BookingSeats).Where(b =>
                        b.Route.FromCity.Trim().ToLower() == fromCity &&
                        b.Route.ToCity.Trim().ToLower() == toCity
                        ).ToListAsync();

            var result = buses.Select(bus =>
            {
                var bookedSeats = bus.Bookings
                    .Where(bk => bk.TravelDate.Date == travelDate.Date && !bk.IsCancelled)
                    .SelectMany(bk => bk.BookingSeats)
                    .Count();

                var available = bus.TotalSeats - bookedSeats;

                return new BusSearchResultDto
                {
                    Id = bus.Id,
                    BusName = bus.BusName,
                    TotalSeats = bus.TotalSeats,
                    AvailableSeats = available,
                    Price = bus.Price,
                    RouteId = bus.RouteId,
                    FromCity = bus.Route.FromCity,
                    ToCity = bus.Route.ToCity
                };
            }).ToList();

            return result;
        }
    }
}
