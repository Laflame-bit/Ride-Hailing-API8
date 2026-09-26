using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RideHailingAPI.DTOs.Ride;
using RideHailingAPI.Services.Interfaces;

namespace RideHailingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RideController  : ControllerBase
{
     private readonly IRideService _rideService;

    public RideController(IRideService rideService)
    {
        _rideService = rideService;
    }

    [HttpPost("request")]
    [Authorize(Roles = "Passenger")]
    public async Task<IActionResult> RequestRide(
        CreateRideRequest request)
    {
        var userId = GetUserId();

        if (userId == null)
            return Unauthorized();

        try
        {
            var ride =
                await _rideService.RequestRideAsync(
                    userId.Value,
                    request);

            return Ok(ride);
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpGet("{rideId}")]
    public async Task<IActionResult> GetRide(
        int rideId)
    {
        var userId = GetUserId();

        if (userId == null)
            return Unauthorized();

        var ride =
            await _rideService.GetByIdAsync(
                rideId,
                userId.Value);

        if (ride == null)
            return NotFound(new
            {
                message = "Ride not found."
            });

        return Ok(ride);
    }

    [HttpGet("my-rides")]
    [Authorize(Roles = "Passenger")]
    public async Task<IActionResult> GetMyRides()
    {
        var userId = GetUserId();

        if (userId == null)
            return Unauthorized();

        var rides =
            await _rideService.GetPassengerRidesAsync(
                userId.Value);

        return Ok(rides);
    }

    [HttpGet("driver-rides")]
    [Authorize(Roles = "Driver")]
    public async Task<IActionResult> GetDriverRides()
    {
        var userId = GetUserId();

        if (userId == null)
            return Unauthorized();

        var rides =
            await _rideService.GetDriverRidesAsync(
                userId.Value);

        return Ok(rides);
    }

    [HttpGet("available")]
    [Authorize(Roles = "Driver")]
    public async Task<IActionResult> GetAvailableRides()
    {
        var rides =
            await _rideService.GetAvailableRidesAsync();

        return Ok(rides);
    }

    [HttpPost("{rideId}/accept")]
    [Authorize(Roles = "Driver")]
    public async Task<IActionResult> AcceptRide(
        int rideId)
    {
        var userId = GetUserId();

        if (userId == null)
            return Unauthorized();

        try
        {
            var ride =
                await _rideService.AcceptRideAsync(
                    rideId,
                    userId.Value);

            if (ride == null)
                return NotFound(new
                {
                    message = "Ride not found."
                });

            return Ok(ride);
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }
    
    [HttpPost("{rideId}/reject")]
    [Authorize(Roles = "Driver")]
    public async Task<IActionResult> RejectRide(int rideId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdClaim, out var driverId))
        {
            return Unauthorized();
        }

        var ride = await _rideService.RejectRideAsync(
            rideId,
            driverId);

        if (ride == null)
        {
            return BadRequest(new
            {
                message = "Ride cannot be rejected."
            });
        }

        return Ok(ride);
    }

    [HttpPut("{rideId}/status")]
    [Authorize(Roles = "Driver")]
    public async Task<IActionResult> UpdateStatus(
        int rideId,
        UpdateRideStatusRequest request)
    {
        var userId = GetUserId();

        if (userId == null)
            return Unauthorized();

        try
        {
            var ride =
                await _rideService.UpdateStatusAsync(
                    rideId,
                    userId.Value,
                    request);

            if (ride == null)
                return NotFound(new
                {
                    message = "Ride not found or you are not the assigned driver."
                });

            return Ok(ride);
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpPost("{rideId}/cancel")]
    [Authorize(Roles = "Passenger")]
    public async Task<IActionResult> CancelRide(
        int rideId,
        CancelRideRequest request)
    {
        var userId = GetUserId();

        if (userId == null)
            return Unauthorized();

        try
        {
            var ride =
                await _rideService.CancelRideAsync(
                    rideId,
                    userId.Value,
                    request);

            if (ride == null)
                return NotFound(new
                {
                    message = "Ride not found or you do not own this ride."
                });

            return Ok(ride);
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpGet("{rideId}/history")]
    public async Task<IActionResult> GetStatusHistory(
        int rideId)
    {
        var userId = GetUserId();

        if (userId == null)
            return Unauthorized();

        var history =
            await _rideService.GetStatusHistoryAsync(
                rideId,
                userId.Value);

        return Ok(history);
    }

    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllRides()
    {
        var rides =
            await _rideService.GetAllAsync();

        return Ok(rides);
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