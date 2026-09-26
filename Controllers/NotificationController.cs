using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RideHailingAPI.Services.Interfaces;

namespace RideHailingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationController(
        INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyNotifications()
    {
        var userId = GetUserId();

        if (userId == null)
            return Unauthorized();

        var notifications =
            await _notificationService
                .GetMyNotificationsAsync(userId.Value);

        return Ok(notifications);
    }

    [HttpPut("{notificationId}/read")]
    public async Task<IActionResult> MarkAsRead(
        int notificationId)
    {
        var userId = GetUserId();

        if (userId == null)
            return Unauthorized();

        var result =
            await _notificationService.MarkAsReadAsync(
                notificationId,
                userId.Value);

        if (!result)
        {
            return NotFound(new
            {
                message = "Notification not found."
            });
        }

        return Ok(new
        {
            message = "Notification marked as read."
        });
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