namespace RideHailingAPI.DTOs.Ride;

public class CreateRideRequest
{
    public string? PickupLocation { get; set; }
    public string? Destination { get; set; }
}