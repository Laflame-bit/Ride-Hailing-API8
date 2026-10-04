using Dapper;
using RideHailingAPI.Data;
using RideHailingAPI.Domain.Entities;
using RideHailingAPI.Repositories.Interfaces;

namespace RideHailingAPI.Repositories.Implementations;

public class DriverProfileRepository : IDriverProfileRepository
{
    private readonly DapperContext _context;

    public DriverProfileRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<DriverProfile?> GetByIdAsync(int id)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
            SELECT
                d.*,
                u.*,
                v.*
            FROM DriverProfiles d
            INNER JOIN Users u ON d.UserId = u.Id
            LEFT JOIN Vehicles v ON d.Id = v.DriverProfileId
            WHERE d.Id = @Id
            """;

        var result = await connection.QueryAsync<DriverProfile, User, Vehicle, DriverProfile>(
            sql,
            (driverProfile, user, vehicle) =>
            {
                driverProfile.User = user;
                driverProfile.Vehicle = vehicle;

                return driverProfile;
            },
            new { Id = id },
            splitOn: "Id,Id"
        );

        return result.FirstOrDefault();
    }

    public async Task<DriverProfile?> GetByUserIdAsync(int userId)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
            SELECT
                d.*,
                u.*,
                v.*
            FROM DriverProfiles d
            INNER JOIN Users u ON d.UserId = u.Id
            LEFT JOIN Vehicles v ON d.Id = v.DriverProfileId
            WHERE d.UserId = @UserId
            """;

        var result = await connection.QueryAsync<DriverProfile, User, Vehicle, DriverProfile>(
            sql,
            (driverProfile, user, vehicle) =>
            {
                driverProfile.User = user;
                driverProfile.Vehicle = vehicle;

                return driverProfile;
            },
            new { UserId = userId },
            splitOn: "Id,Id"
        );

        return result.FirstOrDefault();
    }

    public async Task<List<DriverProfile>> GetAllAsync()
    {
        using var connection = _context.CreateConnection();

        const string sql = """
            SELECT
                d.*,
                u.*,
                v.*
            FROM DriverProfiles d
            INNER JOIN Users u ON d.UserId = u.Id
            LEFT JOIN Vehicles v ON d.Id = v.DriverProfileId
            """;

        var result = await connection.QueryAsync<DriverProfile, User, Vehicle, DriverProfile>(
            sql,
            (driverProfile, user, vehicle) =>
            {
                driverProfile.User = user;
                driverProfile.Vehicle = vehicle;

                return driverProfile;
            },
            splitOn: "Id,Id"
        );

        return result.ToList();
    }

    public async Task<DriverProfile> AddAsync(DriverProfile driverProfile)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
            INSERT INTO DriverProfiles
            (
                UserId,
                LicenseNumber,
                IsApproved,
                IsAvailable,
                CreatedAt
            )
            OUTPUT INSERTED.*
            VALUES
            (
                @UserId,
                @LicenseNumber,
                @IsApproved,
                @IsAvailable,
                @CreatedAt
            )
            """;

        return await connection.QuerySingleAsync<DriverProfile>(
            sql,
            driverProfile
        );
    }

    public async Task UpdateAsync(DriverProfile driverProfile)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
            UPDATE DriverProfiles
            SET
                UserId = @UserId,
                LicenseNumber = @LicenseNumber,
                IsApproved = @IsApproved,
                IsAvailable = @IsAvailable
            WHERE Id = @Id
            """;

        await connection.ExecuteAsync(sql, driverProfile);
    }
}