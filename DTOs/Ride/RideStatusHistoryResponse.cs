using RideHailingAPI.Domain.Enums;

namespace RideHailingAPI.DTOs.Ride;

public class RideStatusHistoryResponse
{
    public int Id { get; set; }
    public int RideId { get; set; }
    public RideStatus Status { get; set; }
    public int ChangedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
}