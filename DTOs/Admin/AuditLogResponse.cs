namespace RideHailingAPI.DTOs.Admin;

public class AuditLogResponse
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public string? Action { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}