using Dapper;
using RideHailingAPI.Data;
using RideHailingAPI.Domain.Entities;
using RideHailingAPI.Repositories.Interfaces;

namespace RideHailingAPI.Repositories.Implementations;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly DapperContext _context;

    public AuditLogRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<List<AuditLog>> GetAllAsync()
    {
        using var connection = _context.CreateConnection();

        const string sql = """
                           SELECT
                               a.*,
                               u.*
                           FROM AuditLogs a
                           LEFT JOIN Users u
                               ON a.UserId = u.Id
                           ORDER BY a.CreatedAt DESC
                           """;

        var result = await connection.QueryAsync<AuditLog, User, AuditLog>(
            sql,
            (auditLog, user) =>
            {
                auditLog.User = user;

                return auditLog;
            },
            splitOn: "Id"
        );

        return result.ToList();
    }

    public async Task<AuditLog> AddAsync(AuditLog auditLog)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
                           INSERT INTO AuditLogs
                           (
                               UserId,
                               Action,
                               Description,
                               CreatedAt
                           )
                           OUTPUT INSERTED.*
                           VALUES
                           (
                               @UserId,
                               @Action,
                               @Description,
                               @CreatedAt
                           )
                           """;

        return await connection.QuerySingleAsync<AuditLog>(
            sql,
            auditLog
        );
    }
}