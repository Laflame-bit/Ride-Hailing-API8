using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RideHailingAPI.DTOs.Driver;
using RideHailingAPI.Services.Interfaces;

namespace RideHailingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DriverController  : ControllerBase
{
    private readonly IDriverService _driverService;

    public DriverController(IDriverService driverService)
    {
        _driverService = driverService;
    }

    [HttpPost("profile")]
    [Authorize(Roles = "Driver")]
    public async Task<IActionResult> CreateProfile(
        DriverProfileRequest request)
    {
        var userId = GetUserId();

        if (userId == null)
            return Unauthorized();

        try
        {
            var profile =
                await _driverService.CreateProfileAsync(
                    userId.Value,
                    request);

            return Ok(profile);
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpGet("profile")]
    [Authorize(Roles = "Driver")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = GetUserId();

        if (userId == null)
            return Unauthorized();

        var profile =
            await _driverService.GetProfileAsync(userId.Value);

        if (profile == null)
            return NotFound(new
            {
                message = "Driver profile not found."
            });

        return Ok(profile);
    }

    [HttpPut("availability")]
    [Authorize(Roles = "Driver")]
    public async Task<IActionResult> UpdateAvailability(
        bool isAvailable)
    {
        var userId = GetUserId();

        if (userId == null)
            return Unauthorized();

        try
        {
            var profile =
                await _driverService.UpdateAvailabilityAsync(
                    userId.Value,
                    isAvailable);

            if (profile == null)
                return NotFound(new
                {
                    message = "Driver profile not found."
                });

            return Ok(profile);
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllDrivers()
    {
        var drivers =
            await _driverService.GetAllAsync();

        return Ok(drivers);
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