using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RideHailingAPI.DTOs.Admin;
using RideHailingAPI.Services.Interfaces;

namespace RideHailingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;
    private readonly IDriverService _driverService;
    private readonly IRideService _rideService;
    private readonly IAuditLogService _auditLogService;

    public AdminController(
        IAdminService adminService,
        IDriverService driverService,
        IRideService rideService,
        IAuditLogService auditLogService)
    {
        _adminService = adminService;
        _driverService = driverService;
        _rideService = rideService;
        _auditLogService = auditLogService;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users =
            await _adminService.GetAllUsersAsync();

        return Ok(users);
    }

    [HttpGet("users/{userId}")]
    public async Task<IActionResult> GetUser(
        int userId)
    {
        var user =
            await _adminService.GetUserByIdAsync(userId);

        if (user == null)
            return NotFound(new
            {
                message = "User not found."
            });

        return Ok(user);
    }

    [HttpPut("users/{userId}/status")]
    public async Task<IActionResult> UpdateUserStatus(
        int userId,
        UpdateUserStatusRequest request)
    {
        var user =
            await _adminService.UpdateUserStatusAsync(
                userId,
                request);

        if (user == null)
            return NotFound(new
            {
                message = "User not found."
            });
        var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(adminIdClaim, out var adminId))
        {
            return Unauthorized();
        }
        await _auditLogService.CreateAsync(
            adminId,
            "UpdateUserStatus",
            $"Admin changed user {userId} active status to {request.IsActive}.");

        return Ok(user);
    }

    [HttpPut("drivers/{driverProfileId}/approval")]
    public async Task<IActionResult> ApproveDriver(
        int driverProfileId,
        DriverApprovalRequest request)
    {
        var driver =
            await _adminService.ApproveDriverAsync(
                driverProfileId,
                request);

        if (driver == null)
            return NotFound(new
            {
                message = "Driver profile not found."
            });
        var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(adminIdClaim, out var adminId))
        {
            return Unauthorized();
        }

        await _auditLogService.CreateAsync(
            adminId,
            "DriverApproval",
            $"Admin changed driver {driverProfileId} approval status to {request.IsApproved}.");

        return Ok(driver);
    }

    [HttpGet("drivers")]
    public async Task<IActionResult> GetAllDrivers()
    {
        var drivers =
            await _driverService.GetAllAsync();

        return Ok(drivers);
    }

    [HttpGet("rides")]
    public async Task<IActionResult> GetAllRides()
    {
        var rides =
            await _rideService.GetAllAsync();

        return Ok(rides);
    }

    [HttpGet("audit-logs")]
    public async Task<IActionResult> GetAuditLogs()
    {
        var logs =
            await _auditLogService.GetAllAsync();

        return Ok(logs);
    }
}