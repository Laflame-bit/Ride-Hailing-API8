using RideHailingAPI.Domain.Entities;
using RideHailingAPI.DTOs.Driver;
using RideHailingAPI.Repositories.Interfaces;
using RideHailingAPI.Services.Interfaces;

namespace RideHailingAPI.Services.Implementations;

public class DriverService  : IDriverService
{
    
    private readonly IDriverProfileRepository _driverProfileRepository;
    private readonly IUserRepository _userRepository;

    public DriverService(
        IDriverProfileRepository driverProfileRepository,
        IUserRepository userRepository)
    {
        _driverProfileRepository = driverProfileRepository;
        _userRepository = userRepository;
    }

    public async Task<DriverProfileResponse> CreateProfileAsync(int userId, DriverProfileRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
            throw new Exception("User not found.");

        var existingProfile =
            await _driverProfileRepository.GetByUserIdAsync(userId);

        if (existingProfile != null)
            throw new Exception("Driver profile already exists.");

        var driverProfile = new DriverProfile
        {
            UserId = userId,
            LicenseNumber = request.LicenseNumber,
            IsApproved = false,
            IsAvailable = false
        };

        await _driverProfileRepository.AddAsync(driverProfile);

        return MapToResponse(driverProfile);
    }

    public async Task<DriverProfileResponse?> GetProfileAsync(int userId)
    {
        var driverProfile =
            await _driverProfileRepository.GetByUserIdAsync(userId);

        if (driverProfile == null)
            return null;

        return MapToResponse(driverProfile);
    }

    public async Task<List<DriverProfileResponse>> GetAllAsync()
    {
        var profiles = await _driverProfileRepository.GetAllAsync();

        return profiles
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<DriverProfileResponse?> UpdateAvailabilityAsync(int userId, bool isAvailable)
    {
        var driverProfile =
            await _driverProfileRepository.GetByUserIdAsync(userId);

        if (driverProfile == null)
            return null;

        if (!driverProfile.IsApproved)
            throw new Exception("Driver has not been approved.");

        driverProfile.IsAvailable = isAvailable;

        await _driverProfileRepository.UpdateAsync(driverProfile);

        return MapToResponse(driverProfile);
    }

    public async Task<DriverProfileResponse?> ApproveDriverAsync(int driverProfileId, bool isApproved)
    {
        var driverProfile =
            await _driverProfileRepository.GetByIdAsync(driverProfileId);

        if (driverProfile == null)
            return null;

        driverProfile.IsApproved = isApproved;

        if (!isApproved)
            driverProfile.IsAvailable = false;

        await _driverProfileRepository.UpdateAsync(driverProfile);

        return MapToResponse(driverProfile);
    }

    private DriverProfileResponse MapToResponse(
        DriverProfile driverProfile)
    {
        return new DriverProfileResponse
        {
            Id = driverProfile.Id,
            UserId = driverProfile.UserId,
            LicenseNumber = driverProfile.LicenseNumber,
            IsApproved = driverProfile.IsApproved,
            IsAvailable = driverProfile.IsAvailable,
            CreatedAt = driverProfile.CreatedAt
        };
    }
}