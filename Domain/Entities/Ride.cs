using RideHailingAPI.Domain.Enums;

namespace RideHailingAPI.Domain.Entities;

public class Ride
{
    public int Id { get; set; }

    public string? RideReference { get; set; }

    public int PassengerId { get; set; }

    public int? DriverId { get; set; }

    public string? PickupLocation { get; set; }

    public string? Destination { get; set; }

    public RideStatus Status { get; set; } = RideStatus.Requested;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User? Passenger { get; set; }

    public User? Driver { get; set; }
}