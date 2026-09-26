using RideHailingAPI.Domain.Enums;

namespace RideHailingAPI.DTOs.Ride;

public class RideResponse
{
    public int Id { get; set; }
    public string? RideReference { get; set; }
    public int PassengerId { get; set; }
    public int? DriverId { get; set; }
    public string? PickupLocation { get; set; }
    public string? Destination { get; set; }
    public RideStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}