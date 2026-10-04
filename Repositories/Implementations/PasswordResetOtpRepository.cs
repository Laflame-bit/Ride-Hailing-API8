using Dapper;
using RideHailingAPI.Data;
using RideHailingAPI.Domain.Entities;
using RideHailingAPI.Repositories.Interfaces;

namespace RideHailingAPI.Repositories.Implementations;

public class PasswordResetOtpRepository : IPasswordResetOtpRepository
{
    private readonly DapperContext _context;

    public PasswordResetOtpRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<PasswordResetOtp?> GetValidOtpAsync(int userId, string otpCode)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
            SELECT *
            FROM PasswordResetOtps
            WHERE UserId = @UserId
              AND OtpCode = @OtpCode
              AND IsUsed = 0
              AND ExpiresAt > @Now
            """;

        return await connection.QueryFirstOrDefaultAsync<PasswordResetOtp>(
            sql,
            new
            {
                UserId = userId,
                OtpCode = otpCode,
                Now = DateTime.UtcNow
            }
        );
    }

    public async Task<PasswordResetOtp?> GetLatestAsync(int userId)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
            SELECT TOP 1 *
            FROM PasswordResetOtps
            WHERE UserId = @UserId
            ORDER BY CreatedAt DESC
            """;

        return await connection.QueryFirstOrDefaultAsync<PasswordResetOtp>(
            sql,
            new { UserId = userId }
        );
    }

    public async Task<PasswordResetOtp> AddAsync(PasswordResetOtp otp)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
            INSERT INTO PasswordResetOtps
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

        return await connection.QuerySingleAsync<PasswordResetOtp>(
            sql,
            otp
        );
    }

    public async Task UpdateAsync(PasswordResetOtp otp)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
            UPDATE PasswordResetOtps
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
            UPDATE PasswordResetOtps
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