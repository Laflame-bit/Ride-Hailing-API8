using RideHailingAPI.Domain.Entities;

namespace RideHailingAPI.Repositories.Interfaces;

public interface IAuditLogRepository
{
    Task<List<AuditLog>> GetAllAsync();
    Task<AuditLog> AddAsync(AuditLog auditLog);
}