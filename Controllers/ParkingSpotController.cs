using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkingManagementSystem.Data;
using ParkingManagementSystem.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
[Route("api/[controller]")]
[ApiController]
[Authorize] // Ensure the user is authenticated
public class ParkingSpotController : ControllerBase
{
    private readonly AppDbContext _context;

    public ParkingSpotController(AppDbContext context)
    {
        _context = context;
    }

    // POST: api/parkingSpots/{spotId}/entry
    [HttpPost("{spotId}/entry")]
    public async Task<IActionResult> SetParkingSpotOccupancy(int spotId, [FromBody] ParkingSpotsModel parkingSpot)
    {
        var username = User.Identity.Name; // Get the username from the JWT token

        // Ensure the parking spot is available
        var spot = await _context.ParkingSpots
                                  .FirstOrDefaultAsync(s => s.SpotId == spotId && !s.IsOccupied);

        if (spot == null)
        {
            return BadRequest("The selected parking spot is not available or already occupied.");
        }

        // Mark the spot as occupied and record the entry details
        spot.IsOccupied = true;
        spot.VehicleOwner = username; // Associate the current user with the vehicle
        spot.VehicleType = parkingSpot.VehicleType; // Set the vehicle type
        spot.StartTime = DateTime.Now; // Set the entry time
        spot.AmountCharged = 0; // Reset amount (will be calculated later)
        spot.VehicleId = parkingSpot.VehicleId;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Vehicle entered the parking spot successfully." });
    }

    // POST: api/parkingSpots/{spotId}/exit
    [HttpPost("{spotId}/exit")]
    public async Task<IActionResult> ExitParkingSpot(int spotId)
    {
        var username = User.Identity.Name; // Get the username from the JWT token

        // Find the occupied parking spot by the user
        var spot = await _context.ParkingSpots
                                  .FirstOrDefaultAsync(s => s.SpotId == spotId && s.VehicleOwner == username && s.IsOccupied);

        if (spot == null)
        {
            return BadRequest("No matching parking spot found for the user or the spot is not occupied.");
        }

        // Calculate the amount for the vehicle
        spot.CalculateAmount();

        // Mark the spot as vacant
        spot.IsOccupied = false;
        spot.VehicleOwner = string.Empty; // Clear owner
        spot.VehicleType = ""; // Reset to default type
        spot.StartTime = DateTime.MinValue; // Reset to default start time
        spot.AmountCharged = 0;  // Reset the amount charged
        spot.VehicleId = 0;
        // Save the updated details
        await _context.SaveChangesAsync();
        // Calculate parked duration
        TimeSpan parkedDuration = (DateTime.Now - spot.StartTime).GetValueOrDefault();

        // Format the duration to hh:mm:ss
        string formattedDuration = parkedDuration.ToString(@"hh\:mm\:ss");
        return Ok(new
        {
            message = "Vehicle exited the parking spot successfully.",
            amountCharged = spot.AmountCharged,
            parkedDuration = spot.StartTime != DateTime.MinValue ? formattedDuration : "N/A"
        });
    }

    // GET: api/parkingSpots/available
    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableParkingSpots()
    {
        var availableSpots = await _context.ParkingSpots
                                           .Where(s => !s.IsOccupied)
                                           .ToListAsync();

        if (!availableSpots.Any())
        {
            return NotFound("No available parking spots.");
        }

        return Ok(availableSpots);
    }

    // GET: api/parkingSpots/history
    [HttpGet("history")]
    public async Task<IActionResult> GetParkingHistory()
    {
        var username = User.Identity.Name; // Get the username from the JWT token

        var history = await _context.ParkingSpots
                                    .Where(s => s.VehicleOwner == username)
                                    .OrderByDescending(s => s.StartTime)
                                    .ToListAsync();

        if (history == null || !history.Any())
        {
            return NotFound("No parking history found.");
        }

        return Ok(history);
    }
}
