using RideHailingAPI.Domain.Entities;

namespace RideHailingAPI.Repositories.Interfaces;

public interface IVehicleRepository
{
    Task<Vehicle?> GetByIdAsync(int id);
    Task<Vehicle?> GetByDriverProfileIdAsync(int driverProfileId);
    Task<List<Vehicle>> GetAllAsync();
    Task<Vehicle> AddAsync(Vehicle vehicle);
    Task UpdateAsync(Vehicle vehicle);
}