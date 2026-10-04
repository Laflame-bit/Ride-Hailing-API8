using Dapper;
using RideHailingAPI.Data;
using RideHailingAPI.Domain.Entities;
using RideHailingAPI.Repositories.Interfaces;

namespace RideHailingAPI.Repositories.Implementations;

public class PhoneOtpRepository : IPhoneOtpRepository
{
    private readonly DapperContext _context;

    public PhoneOtpRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<PhoneOtp?> GetValidOtpAsync(int userId, string otpCode)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
            SELECT *
            FROM PhoneOtps
            WHERE UserId = @UserId
              AND OtpCode = @OtpCode
              AND IsUsed = 0
              AND ExpiresAt > @Now
            """;

        return await connection.QueryFirstOrDefaultAsync<PhoneOtp>(
            sql,
            new
            {
                UserId = userId,
                OtpCode = otpCode,
                Now = DateTime.UtcNow
            }
        );
    }

    public async Task<PhoneOtp?> GetLatestAsync(int userId)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
            SELECT TOP 1 *
            FROM PhoneOtps
            WHERE UserId = @UserId
            ORDER BY CreatedAt DESC
            """;

        return await connection.QueryFirstOrDefaultAsync<PhoneOtp>(
            sql,
            new { UserId = userId }
        );
    }

    public async Task<PhoneOtp> AddAsync(PhoneOtp otp)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
            INSERT INTO PhoneOtps
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

        return await connection.QuerySingleAsync<PhoneOtp>(
            sql,
            otp
        );
    }

    public async Task UpdateAsync(PhoneOtp otp)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
            UPDATE PhoneOtps
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
            UPDATE PhoneOtps
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