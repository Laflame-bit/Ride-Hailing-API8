using RideHailingAPI.DTOs.Admin;

namespace RideHailingAPI.Services.Interfaces;

public interface IAuditLogService
{
    Task<List<AuditLogResponse>> GetAllAsync();

    Task<AuditLogResponse> CreateAsync(
        int? userId,
        string action,
        string description);
}