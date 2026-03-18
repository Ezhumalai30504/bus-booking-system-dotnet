using BusBooking_API.DTOs.RouteDTOs;
using BusBooking_API.Model;
using BusBooking_API.Repositary.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BusBooking_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusRouteController : ControllerBase
    {
        private readonly IRouteRepositary _routeRepo;

        public BusRouteController(IRouteRepositary routeRepo)
        {
            _routeRepo = routeRepo;
        }

        // ✅ Get All Routes
        [Authorize(Roles = "Admin,User")]
        [HttpGet("ReadRoute")]
        public async Task<IActionResult> GetAll()
        {
            var routes = await _routeRepo.GetAllAsync();

            var result = routes.Select(r => new BusRouteReadDto
            {
                Id = r.Id,
                FromCity = r.FromCity,
                ToCity = r.ToCity
            });

            return Ok(result);
        }

        // ✅ Get Route By Id
        [Authorize(Roles = "Admin,User")]
        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var route = await _routeRepo.GetByIdAsync(id);
            if (route == null) return NotFound();

            var dto = new BusRouteReadDto
            {
                Id = route.Id,
                FromCity = route.FromCity,
                ToCity = route.ToCity
            };

            return Ok(dto);
        }

        // ✅ Create Route
        [Authorize(Roles = "Admin")]
        [HttpPost("CreateRoute")]
        public async Task<IActionResult> Create(BusRouteCreateDto dto)
        {
            var route = new BusRoute
            {
                FromCity = dto.FromCity,
                ToCity = dto.ToCity
            };

            await _routeRepo.AddAsync(route);
            return Ok("Route created");
        }

        // ✅ Update Route
        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateRoute/{id}")]
        public async Task<IActionResult> Update(int id, BusRouteUpdateDto dto)
        {
            var route = await _routeRepo.GetByIdAsync(id);
            if (route == null) return NotFound();

            route.FromCity = dto.FromCity;
            route.ToCity = dto.ToCity;

            await _routeRepo.UpdateAsync(route);
            return Ok("Route updated");
        }

        // ✅ Delete Route
        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteRoute/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _routeRepo.DeleteAsync(id);
            return Ok("Route deleted");
        }
    }
}
