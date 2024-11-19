using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkingManagementSystem.Models;
using ParkingManagementSystem.Data; // Assuming ApplicationDbContext is here
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace ParkingManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // This ensures that the user is authenticated
    public class VehicleController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VehicleController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/vehicle/owner
        [HttpGet("owner")]
        public async Task<IActionResult> GetVehiclesByOwner()
        {
            // Extract UserId from the claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("UserId claim is missing in the token.");
            }

            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("Invalid UserId claim.");
            }

            // Assign the logged-in user's details to the vehicle
            var ownerId = userId; // Get the username from the JWT token
            Console.WriteLine("Hello World!");
            // Retrieve all vehicles belonging to the logged-in user
            var vehicles = await _context.Vehicles
                                          .Where(v => v.UserId == ownerId)
                                          .ToListAsync();

            if (vehicles == null || !vehicles.Any())
            {
                return NotFound("No vehicles found for the user.");
            }

            return Ok(vehicles);
        }

        // GET: api/vehicle/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVehicleById(int id)
        {
            var vehicle = await _context.Vehicles
                                         .FirstOrDefaultAsync(v => v.VehicleId == id);

            if (vehicle == null)
            {
                return NotFound("Vehicle not found.");
            }

            return Ok(vehicle);
        }

        // POST: api/vehicle
        [HttpPost]
        public async Task<IActionResult> AddVehicle([FromBody] Vehicle vehicle)
        {
            if (vehicle == null)
            {
                return BadRequest("Vehicle data is required.");
            }

            // Extract UserId from the claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("UserId claim is missing in the token.");
            }

            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("Invalid UserId claim.");
            }

            // Assign the logged-in user's details to the vehicle
            vehicle.UserId = userId; // Assign the UserId from claims
            // Assign the username from claims

            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVehicleById), new { id = vehicle.VehicleId }, vehicle);
        }


    }
}
