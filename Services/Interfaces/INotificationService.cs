using RideHailingAPI.DTOs.Notification;

namespace RideHailingAPI.Services.Interfaces;

public interface INotificationService
{
    Task<List<NotificationResponse>> GetMyNotificationsAsync(int userId);

    Task<NotificationResponse> CreateAsync(
        int userId,
        string title,
        string message);

    Task<bool> MarkAsReadAsync(
        int notificationId,
        int userId);
}