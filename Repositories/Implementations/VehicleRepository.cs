using Dapper;
using RideHailingAPI.Data;
using RideHailingAPI.Domain.Entities;
using RideHailingAPI.Repositories.Interfaces;

namespace RideHailingAPI.Repositories.Implementations;

public class VehicleRepository : IVehicleRepository
{
    private readonly DapperContext _context;

    public VehicleRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<Vehicle?> GetByIdAsync(int id)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
            SELECT
                v.*,
                d.*
            FROM Vehicles v
            INNER JOIN DriverProfiles d
                ON v.DriverProfileId = d.Id
            WHERE v.Id = @Id
            """;

        var result = await connection.QueryAsync<Vehicle, DriverProfile, Vehicle>(
            sql,
            (vehicle, driverProfile) =>
            {
                vehicle.DriverProfile = driverProfile;

                return vehicle;
            },
            new { Id = id },
            splitOn: "Id"
        );

        return result.FirstOrDefault();
    }

    public async Task<Vehicle?> GetByDriverProfileIdAsync(int driverProfileId)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
            SELECT
                v.*,
                d.*
            FROM Vehicles v
            INNER JOIN DriverProfiles d
                ON v.DriverProfileId = d.Id
            WHERE v.DriverProfileId = @DriverProfileId
            """;

        var result = await connection.QueryAsync<Vehicle, DriverProfile, Vehicle>(
            sql,
            (vehicle, driverProfile) =>
            {
                vehicle.DriverProfile = driverProfile;

                return vehicle;
            },
            new { DriverProfileId = driverProfileId },
            splitOn: "Id"
        );

        return result.FirstOrDefault();
    }

    public async Task<List<Vehicle>> GetAllAsync()
    {
        using var connection = _context.CreateConnection();

        const string sql = """
            SELECT
                v.*,
                d.*
            FROM Vehicles v
            INNER JOIN DriverProfiles d
                ON v.DriverProfileId = d.Id
            """;

        var result = await connection.QueryAsync<Vehicle, DriverProfile, Vehicle>(
            sql,
            (vehicle, driverProfile) =>
            {
                vehicle.DriverProfile = driverProfile;

                return vehicle;
            },
            splitOn: "Id"
        );

        return result.ToList();
    }

    public async Task<Vehicle> AddAsync(Vehicle vehicle)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
            INSERT INTO Vehicles
            (
                DriverProfileId,
                Make,
                Model,
                PlateNumber,
                Color,
                CreatedAt
            )
            OUTPUT INSERTED.*
            VALUES
            (
                @DriverProfileId,
                @Make,
                @Model,
                @PlateNumber,
                @Color,
                @CreatedAt
            )
            """;

        return await connection.QuerySingleAsync<Vehicle>(
            sql,
            vehicle
        );
    }

    public async Task UpdateAsync(Vehicle vehicle)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
            UPDATE Vehicles
            SET
                DriverProfileId = @DriverProfileId,
                Make = @Make,
                Model = @Model,
                PlateNumber = @PlateNumber,
                Color = @Color
            WHERE Id = @Id
            """;

        await connection.ExecuteAsync(sql, vehicle);
    }
}