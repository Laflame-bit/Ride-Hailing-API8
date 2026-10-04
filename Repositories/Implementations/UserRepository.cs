using Dapper;
using RideHailingAPI.Data;
using RideHailingAPI.Domain.Entities;
using RideHailingAPI.Repositories.Interfaces;

namespace RideHailingAPI.Repositories.Implementations;

public class UserRepository : IUserRepository
{
    private readonly DapperContext _context;

    public UserRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
            SELECT *
            FROM Users
            WHERE Id = @Id
            """;

        return await connection.QueryFirstOrDefaultAsync<User>(
            sql,
            new { Id = id });
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
            SELECT *
            FROM Users
            WHERE Email = @Email
            """;

        return await connection.QueryFirstOrDefaultAsync<User>(
            sql,
            new { Email = email });
    }

    public async Task<User?> GetByPhoneNumberAsync(string phoneNumber)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
            SELECT *
            FROM Users
            WHERE PhoneNumber = @PhoneNumber
            """;

        return await connection.QueryFirstOrDefaultAsync<User>(
            sql,
            new { PhoneNumber = phoneNumber });
    }

    public async Task<List<User>> GetAllAsync()
    {
        using var connection = _context.CreateConnection();

        const string sql = """
                           SELECT *
                           FROM Users
                           """;

        var users = await connection.QueryAsync<User>(sql);

        return users.ToList();
    }

    public async Task<User> AddAsync(User user)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
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
            OUTPUT INSERTED.*
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

        return await connection.QuerySingleAsync<User>(sql, user);
    }

    public async Task UpdateAsync(User user)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
            UPDATE Users
            SET
                FullName = @FullName,
                Email = @Email,
                PhoneNumber = @PhoneNumber,
                PasswordHash = @PasswordHash,
                Role = @Role,
                IsEmailVerified = @IsEmailVerified,
                IsPhoneVerified = @IsPhoneVerified,
                IsActive = @IsActive
            WHERE Id = @Id
            """;

        await connection.ExecuteAsync(sql, user);
    }
}