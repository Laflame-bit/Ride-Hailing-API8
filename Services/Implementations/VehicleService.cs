using RideHailingAPI.Domain.Entities;
using RideHailingAPI.DTOs.Vehicle;
using RideHailingAPI.Repositories.Interfaces;
using RideHailingAPI.Services.Interfaces;

namespace RideHailingAPI.Services.Implementations;

public class VehicleService : IVehicleService
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IDriverProfileRepository _driverProfileRepository;

    public VehicleService(
        IVehicleRepository vehicleRepository,
        IDriverProfileRepository driverProfileRepository)
    {
        _vehicleRepository = vehicleRepository;
        _driverProfileRepository = driverProfileRepository;
    }
    
    public async Task<VehicleResponse> AddVehicleAsync(int userId, VehicleRequest request)
    {
        var driverProfile =
            await _driverProfileRepository.GetByUserIdAsync(userId);

        if (driverProfile == null)
            throw new Exception("Driver profile not found.");

        if (!driverProfile.IsApproved)
            throw new Exception("Driver has not been approved.");

        var existingVehicle =
            await _vehicleRepository.GetByDriverProfileIdAsync(
                driverProfile.Id);

        if (existingVehicle != null)
            throw new Exception("Vehicle already exists.");

        var vehicle = new Vehicle
        {
            DriverProfileId = driverProfile.Id,
            Make = request.Make,
            Model = request.Model,
            PlateNumber = request.PlateNumber,
            Color = request.Color
        };

        await _vehicleRepository.AddAsync(vehicle);

        return MapToResponse(vehicle);
    }

    public async Task<VehicleResponse?> GetByDriverAsync(int userId)
    {
        var driverProfile =
            await _driverProfileRepository.GetByUserIdAsync(userId);

        if (driverProfile == null)
            return null;

        var vehicle =
            await _vehicleRepository.GetByDriverProfileIdAsync(
                driverProfile.Id);

        if (vehicle == null)
            return null;

        return MapToResponse(vehicle);
    }

    public async Task<VehicleResponse?> UpdateVehicleAsync(int userId, VehicleRequest request)
    {
        var driverProfile =
            await _driverProfileRepository.GetByUserIdAsync(userId);

        if (driverProfile == null)
            return null;

        var vehicle =
            await _vehicleRepository.GetByDriverProfileIdAsync(
                driverProfile.Id);

        if (vehicle == null)
            return null;

        vehicle.Make = request.Make;
        vehicle.Model = request.Model;
        vehicle.PlateNumber = request.PlateNumber;
        vehicle.Color = request.Color;

        await _vehicleRepository.UpdateAsync(vehicle);

        return MapToResponse(vehicle);
    }
    private VehicleResponse MapToResponse(Vehicle vehicle)
    {
        return new VehicleResponse
        {
            Id = vehicle.Id,
            DriverProfileId = vehicle.DriverProfileId,
            Make = vehicle.Make,
            Model = vehicle.Model,
            PlateNumber = vehicle.PlateNumber,
            Color = vehicle.Color,
            CreatedAt = vehicle.CreatedAt
        };
    }
}