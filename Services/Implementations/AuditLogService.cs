using RideHailingAPI.Domain.Entities;
using RideHailingAPI.DTOs.Admin;
using RideHailingAPI.Repositories.Interfaces;
using RideHailingAPI.Services.Interfaces;

namespace RideHailingAPI.Services.Implementations;

public class AuditLogService : IAuditLogService
{
    private readonly IAuditLogRepository _auditLogRepository;

    public AuditLogService(
        IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task<List<AuditLogResponse>> GetAllAsync()
    {
        var auditLogs =
            await _auditLogRepository.GetAllAsync();

        return auditLogs
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<AuditLogResponse> CreateAsync(int? userId, string action, string description)
    {
        var auditLog = new AuditLog
        {
            UserId = userId,
            Action = action,
            Description = description
        };

        await _auditLogRepository.AddAsync(auditLog);

        return MapToResponse(auditLog);
    }
    private AuditLogResponse MapToResponse(
        AuditLog auditLog)
    {
        return new AuditLogResponse
        {
            Id = auditLog.Id,
            UserId = auditLog.UserId,
            Action = auditLog.Action,
            Description = auditLog.Description,
            CreatedAt = auditLog.CreatedAt
        };
    }
}