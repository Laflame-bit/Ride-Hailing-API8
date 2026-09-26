using RideHailingAPI.DTOs.Admin;
using RideHailingAPI.DTOs.Driver;
using RideHailingAPI.Repositories.Interfaces;
using RideHailingAPI.Services.Interfaces;

namespace RideHailingAPI.Services.Implementations;

public class AdminService  : IAdminService
{
    private readonly IUserRepository _userRepository;
    private readonly IDriverProfileRepository _driverProfileRepository;

    public AdminService(
        IUserRepository userRepository,
        IDriverProfileRepository driverProfileRepository)
    {
        _userRepository = userRepository;
        _driverProfileRepository = driverProfileRepository;
    }
    
    public async Task<List<AdminUserResponse>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();

        return users
            .Select(user => new AdminUserResponse
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                IsEmailVerified = user.IsEmailVerified,
                IsPhoneVerified = user.IsPhoneVerified,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            })
            .ToList();
    }

    public async Task<AdminUserResponse?> GetUserByIdAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
            return null;

        return new AdminUserResponse
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role,
            IsEmailVerified = user.IsEmailVerified,
            IsPhoneVerified = user.IsPhoneVerified,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<AdminUserResponse?> UpdateUserStatusAsync(int userId, UpdateUserStatusRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
            return null;

        user.IsActive = request.IsActive;

        await _userRepository.UpdateAsync(user);

        return new AdminUserResponse
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role,
            IsEmailVerified = user.IsEmailVerified,
            IsPhoneVerified = user.IsPhoneVerified,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<DriverProfileResponse?> ApproveDriverAsync(int driverProfileId, DriverApprovalRequest request)
    {
        var driverProfile =
            await _driverProfileRepository.GetByIdAsync(
                driverProfileId);

        if (driverProfile == null)
            return null;

        driverProfile.IsApproved = request.IsApproved;

        if (!request.IsApproved)
            driverProfile.IsAvailable = false;

        await _driverProfileRepository.UpdateAsync(driverProfile);

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