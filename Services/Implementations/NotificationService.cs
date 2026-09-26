using RideHailingAPI.Domain.Entities;
using RideHailingAPI.DTOs.Notification;
using RideHailingAPI.Repositories.Interfaces;
using RideHailingAPI.Services.Interfaces;

namespace RideHailingAPI.Services.Implementations;

public class NotificationService  : INotificationService
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationService(
        INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }
    
    public async Task<List<NotificationResponse>> GetMyNotificationsAsync(int userId)
    {
        var notifications =
            await _notificationRepository.GetByUserIdAsync(userId);

        return notifications
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<NotificationResponse> CreateAsync(int userId, string title, string message)
    {
        var notification = new Notification
        {
            UserId = userId,
            Title = title,
            Message = message,
            IsRead = false
        };

        await _notificationRepository.AddAsync(notification);

        return MapToResponse(notification);
    }

    public async Task<bool> MarkAsReadAsync(int notificationId, int userId)
    {
        var notifications =
            await _notificationRepository.GetByUserIdAsync(userId);

        var notification = notifications
            .FirstOrDefault(n => n.Id == notificationId);

        if (notification == null)
            return false;

        notification.IsRead = true;

        await _notificationRepository.UpdateAsync(notification);

        return true;
    }
    
    private NotificationResponse MapToResponse(
        Notification notification)
    {
        return new NotificationResponse
        {
            Id = notification.Id,
            UserId = notification.UserId,
            Title = notification.Title,
            Message = notification.Message,
            IsRead = notification.IsRead,
            CreatedAt = notification.CreatedAt
        };
    }
}