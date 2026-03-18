using BusBooking_API.DTOs.BusDTOs;
using BusBooking_API.Model;
using BusBooking_API.Repositary.Implementations;
using BusBooking_API.Repositary.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace BusBooking_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusController : ControllerBase
    {
        private readonly IBusRepositary _busRepositary;

        private readonly IBusSearchRepositary _searchbusRepository;

        public BusController(IBusRepositary busRepositary, IBusSearchRepositary searchbusRepository)
        {
            _busRepositary = busRepositary;
            _searchbusRepository = searchbusRepository;
        }

        [Authorize(Roles = "User")]
        [HttpGet("search")]
        public async Task<IActionResult> Search(string from, string to, DateTime date)
        {
            if (date.Date < DateTime.Today)
            {
                return BadRequest("Past dates are not allowed.");
            }

            var buses = await _searchbusRepository.SearchBusesAsync(from, to, date);

            return Ok(buses);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("ReadBus")]
        public async Task<IActionResult> GetAll()
        {
            var buses = await _busRepositary.GetAllAsync();

            var result = buses.Select(b => new BusReadDto
            {
              Id = b.Id,
              BusName = b.BusName,
              TotalSeats = b.TotalSeats,
              Price = b.Price,
              RouteId = b.RouteId,
            });

            return Ok(result);
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var bus = await _busRepositary.GetByIdAsync(id);

            if(bus == null)
            {
                return NotFound();
            }

            var dto = new BusReadDto
            {
                Id = bus.Id,
                BusName = bus.BusName,
                TotalSeats = bus.TotalSeats,
                Price = bus.Price,
                RouteId = bus.RouteId,
            };

            return Ok(dto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("Add")]
        public async Task<IActionResult> Add(BusCreateDto dto)
        {
            var bus = new Bus
            {
                BusName = dto.BusName,
                TotalSeats = dto.TotalSeats,
                Price = dto.Price,
                RouteId = dto.RouteId,
            };
            await _busRepositary.AddAsync(bus);
            return Ok(bus);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("update")]
        public async Task<IActionResult> Update(BusUpdateDto dto)
        {
            var bus = new Bus
            {
                Id = dto.Id,
                BusName = dto.BusName,
                TotalSeats = dto.TotalSeats,
                Price = dto.Price,
                RouteId = dto.RouteId,
            };
            _busRepositary.Update(bus);

            return Ok(bus);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var bus = await _busRepositary.GetByIdAsync(id);

            if( bus == null )
            {
                return NotFound();
            }

           await _busRepositary.Delete(bus);

            return Ok("Deleted Successfully");
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet("byRoute/{routeId}")]
        public async Task<IActionResult> GetBusesByRoute(int routeId, DateTime travelDate)
        {

            if (travelDate.Date < DateTime.Today)
            {
                return BadRequest("Past dates are not allowed.");
            }

            var buses = await _busRepositary.GetByRouteAndDateAsync(routeId, travelDate);

            var result = buses.Select(b => new BusReadDto
            {
                Id = b.Id,
                BusName = b.BusName,
                TotalSeats = b.TotalSeats,
                Price = b.Price,
                RouteId = b.RouteId
            });

            return Ok(result);
        }
    }
}
