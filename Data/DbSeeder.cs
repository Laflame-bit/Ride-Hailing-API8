using Dapper;
using RideHailingAPI.Domain.Entities;
using RideHailingAPI.Domain.Enums;

namespace RideHailingAPI.Data;

public static class DbSeeder
{
    public static async Task SeedAdminAsync(DapperContext context)
    {
        using var connection = context.CreateConnection();

        const string checkSql = """
            SELECT COUNT(1)
            FROM Users
            WHERE Role = @Role
            """;

        var adminExists = await connection.ExecuteScalarAsync<int>(
            checkSql,
            new { Role = UserRole.Admin.ToString() }
        );

        if (adminExists > 0)
            return;

        var admin = new User
        {
            FullName = "Olaoye Muhammed",
            Email = "admin@ridehailing.com",
            PhoneNumber = "08000000000",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@888!"),
            Role = UserRole.Admin,
            IsEmailVerified = true,
            IsPhoneVerified = true,
            IsActive = true
        };

        const string insertSql = """
            INSERT INTO Users
            (
                FullName,
                Email,
                PhoneNumber,
                PasswordHash,
                Role,
                IsEmailVerified,
                IsPhoneVerified,
                IsActive,
                CreatedAt
            )
            VALUES
            (
                @FullName,
                @Email,
                @PhoneNumber,
                @PasswordHash,
                @Role,
                @IsEmailVerified,
                @IsPhoneVerified,
                @IsActive,
                @CreatedAt
            )
            """;

        await connection.ExecuteAsync(
            insertSql,
            new
            {
                admin.FullName,
                admin.Email,
                admin.PhoneNumber,
                admin.PasswordHash,
                Role = admin.Role.ToString(),
                admin.IsEmailVerified,
                admin.IsPhoneVerified,
                admin.IsActive,
                admin.CreatedAt
            }
        );
    }
}