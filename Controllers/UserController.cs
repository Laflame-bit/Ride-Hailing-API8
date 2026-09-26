using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RideHailingAPI.DTOs.User;
using RideHailingAPI.Services.Interfaces;

namespace RideHailingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController  : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = GetUserId();

        if (userId == null)
            return Unauthorized();

        var user =
            await _userService.GetByIdAsync(userId.Value);

        if (user == null)
            return NotFound(new
            {
                message = "User not found."
            });

        return Ok(user);
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile(
        UpdateUserRequest request)
    {
        var userId = GetUserId();

        if (userId == null)
            return Unauthorized();

        var user =
            await _userService.UpdateProfileAsync(
                userId.Value,
                request);

        if (user == null)
            return NotFound(new
            {
                message = "User not found."
            });

        return Ok(user);
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