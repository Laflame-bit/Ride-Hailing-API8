using RideHailingAPI.Domain.Entities;

namespace RideHailingAPI.Repositories.Interfaces;

public interface INotificationRepository
{
    Task<List<Notification>> GetByUserIdAsync(int userId);
    Task<Notification> AddAsync(Notification notification);
    Task UpdateAsync(Notification notification);
}