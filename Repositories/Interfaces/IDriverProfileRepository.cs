using RideHailingAPI.Domain.Entities;

namespace RideHailingAPI.Repositories.Interfaces;

public interface IDriverProfileRepository
{
    Task<DriverProfile?> GetByIdAsync(int id);
    Task<DriverProfile?> GetByUserIdAsync(int userId);
    Task<List<DriverProfile>> GetAllAsync();
    Task<DriverProfile> AddAsync(DriverProfile driverProfile);
    Task UpdateAsync(DriverProfile driverProfile);
}