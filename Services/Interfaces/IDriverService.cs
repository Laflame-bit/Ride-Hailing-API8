using RideHailingAPI.DTOs.Driver;

namespace RideHailingAPI.Services.Interfaces;

public interface IDriverService
{
    Task<DriverProfileResponse> CreateProfileAsync(
        int userId,
        DriverProfileRequest request);

    Task<DriverProfileResponse?> GetProfileAsync(int userId);

    Task<List<DriverProfileResponse>> GetAllAsync();

    Task<DriverProfileResponse?> UpdateAvailabilityAsync(
        int userId,
        bool isAvailable);

    Task<DriverProfileResponse?> ApproveDriverAsync(
        int driverProfileId,
        bool isApproved); 
}