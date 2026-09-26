namespace RideHailingAPI.DTOs.Vehicle;

public class VehicleResponse
{
    public int Id { get; set; }
    public int DriverProfileId { get; set; }
    public string? Make { get; set; }
    public string? Model { get; set; }
    public string? PlateNumber { get; set; }
    public string? Color { get; set; }
    public DateTime CreatedAt { get; set; }
}