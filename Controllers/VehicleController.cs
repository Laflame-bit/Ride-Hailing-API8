using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RideHailingAPI.DTOs.Vehicle;
using RideHailingAPI.Services.Interfaces;

namespace RideHailingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Driver")]
public class VehicleController  : ControllerBase
{
    private readonly IVehicleService _vehicleService;

    public VehicleController(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    [HttpPost]
    public async Task<IActionResult> AddVehicle(
        VehicleRequest request)
    {
        var userId = GetUserId();

        if (userId == null)
            return Unauthorized();

        try
        {
            var vehicle =
                await _vehicleService.AddVehicleAsync(
                    userId.Value,
                    request);

            return Ok(vehicle);
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetVehicle()
    {
        var userId = GetUserId();

        if (userId == null)
            return Unauthorized();

        var vehicle =
            await _vehicleService.GetByDriverAsync(
                userId.Value);

        if (vehicle == null)
            return NotFound(new
            {
                message = "Vehicle not found."
            });

        return Ok(vehicle);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateVehicle(
        VehicleRequest request)
    {
        var userId = GetUserId();

        if (userId == null)
            return Unauthorized();

        var vehicle =
            await _vehicleService.UpdateVehicleAsync(
                userId.Value,
                request);

        if (vehicle == null)
            return NotFound(new
            {
                message = "Vehicle not found."
            });

        return Ok(vehicle);
    }

    private int? GetUserId()
    {
        var claim =
            User.FindFirst(ClaimTypes.NameIdentifier);

        if (claim == null)
            return null;

        if (!int.TryParse(claim.Value, out var userId))
            return null;

        return userId;
    }
}