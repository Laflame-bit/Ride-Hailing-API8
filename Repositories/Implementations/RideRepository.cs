using Dapper;
using RideHailingAPI.Data;
using RideHailingAPI.Domain.Entities;
using RideHailingAPI.Domain.Enums;
using RideHailingAPI.Repositories.Interfaces;

namespace RideHailingAPI.Repositories.Implementations;

public class RideRepository : IRideRepository
{
    private readonly DapperContext _context;

    public RideRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<Ride?> GetByIdAsync(int id)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
            SELECT *
            FROM Rides
            WHERE Id = @Id;

            SELECT *
            FROM Users
            WHERE Id = (
                SELECT PassengerId
                FROM Rides
                WHERE Id = @Id
            );

            SELECT *
            FROM Users
            WHERE Id = (
                SELECT DriverId
                FROM Rides
                WHERE Id = @Id
            );
            """;

        using var multi = await connection.QueryMultipleAsync(
            sql,
            new { Id = id }
        );

        var ride = await multi.ReadSingleOrDefaultAsync<Ride>();

        if (ride == null)
            return null;

        ride.Passenger = await multi.ReadSingleOrDefaultAsync<User>();
        ride.Driver = await multi.ReadSingleOrDefaultAsync<User>();

        return ride;
    }

    public async Task<Ride?> GetByReferenceAsync(string rideReference)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
            SELECT *
            FROM Rides
            WHERE RideReference = @RideReference;

            SELECT *
            FROM Users
            WHERE Id = (
                SELECT PassengerId
                FROM Rides
                WHERE RideReference = @RideReference
            );

            SELECT *
            FROM Users
            WHERE Id = (
                SELECT DriverId
                FROM Rides
                WHERE RideReference = @RideReference
            );
            """;

        using var multi = await connection.QueryMultipleAsync(
            sql,
            new { RideReference = rideReference }
        );

        var ride = await multi.ReadSingleOrDefaultAsync<Ride>();

        if (ride == null)
            return null;

        ride.Passenger = await multi.ReadSingleOrDefaultAsync<User>();
        ride.Driver = await multi.ReadSingleOrDefaultAsync<User>();

        return ride;
    }

    public async Task<List<Ride>> GetAllAsync()
    {
        using var connection = _context.CreateConnection();

        const string sql = """
            SELECT *
            FROM Rides
            """;

        var rides = await connection.QueryAsync<Ride>(sql);

        return rides.ToList();
    }

    public async Task<List<Ride>> GetByPassengerIdAsync(int passengerId)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
            SELECT *
            FROM Rides
            WHERE PassengerId = @PassengerId
            """;

        var rides = await connection.QueryAsync<Ride>(
            sql,
            new { PassengerId = passengerId }
        );

        return rides.ToList();
    }

    public async Task<List<Ride>> GetByDriverIdAsync(int driverId)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
            SELECT *
            FROM Rides
            WHERE DriverId = @DriverId
            """;

        var rides = await connection.QueryAsync<Ride>(
            sql,
            new { DriverId = driverId }
        );

        return rides.ToList();
    }

    public async Task<List<Ride>> GetAvailableRidesAsync()
    {
        using var connection = _context.CreateConnection();

        const string sql = """
            SELECT *
            FROM Rides
            WHERE Status = @Status
            AND DriverId IS NULL
            """;

        var rides = await connection.QueryAsync<Ride>(
            sql,
            new { Status = RideStatus.Requested.ToString() }
        );

        return rides.ToList();
    }

    public async Task<Ride> AddAsync(Ride ride)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
            INSERT INTO Rides
            (
                RideReference,
                PassengerId,
                DriverId,
                PickupLocation,
                Destination,
                Status,
                CreatedAt
            )
            OUTPUT INSERTED.*
            VALUES
            (
                @RideReference,
                @PassengerId,
                @DriverId,
                @PickupLocation,
                @Destination,
                @Status,
                @CreatedAt
            )
            """;

        var parameters = new
        {
            ride.RideReference,
            ride.PassengerId,
            ride.DriverId,
            ride.PickupLocation,
            ride.Destination,
            Status = ride.Status.ToString(),
            ride.CreatedAt
        };

        return await connection.QuerySingleAsync<Ride>(
            sql,
            parameters
        );
    }

    public async Task UpdateAsync(Ride ride)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
            UPDATE Rides
            SET
                RideReference = @RideReference,
                PassengerId = @PassengerId,
                DriverId = @DriverId,
                PickupLocation = @PickupLocation,
                Destination = @Destination,
                Status = @Status
            WHERE Id = @Id
            """;

        var parameters = new
        {
            ride.Id,
            ride.RideReference,
            ride.PassengerId,
            ride.DriverId,
            ride.PickupLocation,
            ride.Destination,
            Status = ride.Status.ToString()
        };

        await connection.ExecuteAsync(sql, parameters);
    }
}