using Dapper;
using RideHailingAPI.Data;
using RideHailingAPI.Domain.Entities;
using RideHailingAPI.Repositories.Interfaces;

namespace RideHailingAPI.Repositories.Implementations;

public class NotificationRepository : INotificationRepository
{
    private readonly DapperContext _context;

    public NotificationRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<List<Notification>> GetByUserIdAsync(int userId)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
            SELECT *
            FROM Notifications
            WHERE UserId = @UserId
            ORDER BY CreatedAt DESC
            """;

        var notifications = await connection.QueryAsync<Notification>(
            sql,
            new { UserId = userId }
        );

        return notifications.ToList();
    }

    public async Task<Notification> AddAsync(Notification notification)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
            INSERT INTO Notifications
            (
                UserId,
                Title,
                Message,
                IsRead,
                CreatedAt
            )
            OUTPUT INSERTED.*
            VALUES
            (
                @UserId,
                @Title,
                @Message,
                @IsRead,
                @CreatedAt
            )
            """;

        return await connection.QuerySingleAsync<Notification>(
            sql,
            notification
        );
    }

    public async Task UpdateAsync(Notification notification)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
            UPDATE Notifications
            SET
                UserId = @UserId,
                Title = @Title,
                Message = @Message,
                IsRead = @IsRead
            WHERE Id = @Id
            """;

        await connection.ExecuteAsync(
            sql,
            notification
        );
    }
}