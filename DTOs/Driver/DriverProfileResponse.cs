namespace RideHailingAPI.DTOs.Driver;

public class DriverProfileResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? LicenseNumber { get; set; }
    public bool IsApproved { get; set; }
    public bool IsAvailable { get; set; }
    public DateTime CreatedAt { get; set; }
}