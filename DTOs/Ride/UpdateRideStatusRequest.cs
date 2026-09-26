using RideHailingAPI.Domain.Enums;

namespace RideHailingAPI.DTOs.Ride;

public class UpdateRideStatusRequest
{
    public RideStatus Status { get; set; }
}