using Dapper;
using RideHailingAPI.Data;
using RideHailingAPI.Domain.Entities;
using RideHailingAPI.Repositories.Interfaces;

namespace RideHailingAPI.Repositories.Implementations;

public class RideStatusHistoryRepository : IRideStatusHistoryRepository
{
    private readonly DapperContext _context;

    public RideStatusHistoryRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<List<RideStatusHistory>> GetByRideIdAsync(int rideId)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
                           SELECT *
                           FROM RideStatusHistories
                           WHERE RideId = @RideId
                           ORDER BY CreatedAt
                           """;

        var history = await connection.QueryAsync<RideStatusHistory>(
            sql,
            new { RideId = rideId }
        );

        return history.ToList();
    }

    public async Task<RideStatusHistory> AddAsync(RideStatusHistory history)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
                           INSERT INTO RideStatusHistories
                           (
                               RideId,
                               Status,
                               ChangedByUserId,
                               CreatedAt
                           )
                           OUTPUT INSERTED.*
                           VALUES
                           (
                               @RideId,
                               @Status,
                               @ChangedByUserId,
                               @CreatedAt
                           )
                           """;

        var parameters = new
        {
            history.RideId,
            Status = history.Status.ToString(),
            history.ChangedByUserId,
            history.CreatedAt
        };

        return await connection.QuerySingleAsync<RideStatusHistory>(
            sql,
            parameters
        );
    }
}