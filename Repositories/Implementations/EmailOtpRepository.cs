using Dapper;
using RideHailingAPI.Data;
using RideHailingAPI.Domain.Entities;
using RideHailingAPI.Repositories.Interfaces;

namespace RideHailingAPI.Repositories.Implementations;

public class EmailOtpRepository : IEmailOtpRepository
{
    private readonly DapperContext _context;

    public EmailOtpRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<EmailOtp?> GetValidOtpAsync(int userId, string otpCode)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
            SELECT *
            FROM EmailOtps
            WHERE UserId = @UserId
              AND OtpCode = @OtpCode
              AND IsUsed = 0
              AND ExpiresAt > @Now
            """;

        return await connection.QueryFirstOrDefaultAsync<EmailOtp>(
            sql,
            new
            {
                UserId = userId,
                OtpCode = otpCode,
                Now = DateTime.UtcNow
            }
        );
    }

    public async Task<EmailOtp?> GetLatestAsync(int userId)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
            SELECT TOP 1 *
            FROM EmailOtps
            WHERE UserId = @UserId
            ORDER BY CreatedAt DESC
            """;

        return await connection.QueryFirstOrDefaultAsync<EmailOtp>(
            sql,
            new { UserId = userId }
        );
    }

    public async Task<EmailOtp> AddAsync(EmailOtp otp)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
            INSERT INTO EmailOtps
            (
                UserId,
                OtpCode,
                ExpiresAt,
                IsUsed,
                CreatedAt
            )
            OUTPUT INSERTED.*
            VALUES
            (
                @UserId,
                @OtpCode,
                @ExpiresAt,
                @IsUsed,
                @CreatedAt
            )
            """;

        return await connection.QuerySingleAsync<EmailOtp>(
            sql,
            otp
        );
    }

    public async Task UpdateAsync(EmailOtp otp)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
            UPDATE EmailOtps
            SET
                UserId = @UserId,
                OtpCode = @OtpCode,
                ExpiresAt = @ExpiresAt,
                IsUsed = @IsUsed
            WHERE Id = @Id
            """;

        await connection.ExecuteAsync(sql, otp);
    }

    public async Task InvalidateAllAsync(int userId)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
            UPDATE EmailOtps
            SET IsUsed = 1
            WHERE UserId = @UserId
              AND IsUsed = 0
            """;

        await connection.ExecuteAsync(
            sql,
            new { UserId = userId }
        );
    }
}