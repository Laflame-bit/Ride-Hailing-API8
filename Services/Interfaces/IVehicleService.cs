using RideHailingAPI.DTOs.Vehicle;

namespace RideHailingAPI.Services.Interfaces;

public interface IVehicleService
{
    Task<VehicleResponse> AddVehicleAsync(
        int userId,
        VehicleRequest request);

    Task<VehicleResponse?> GetByDriverAsync(int userId);

    Task<VehicleResponse?> UpdateVehicleAsync(
        int userId,
        VehicleRequest request);
}