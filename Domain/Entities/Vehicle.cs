namespace RideHailingAPI.Domain.Entities;

public class Vehicle
{
    public int Id { get; set; }

    public int DriverProfileId { get; set; }

    public string? Make { get; set; }

    public string? Model { get; set; }

    public string? PlateNumber { get; set; }

    public string? Color { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DriverProfile? DriverProfile { get; set; }
}