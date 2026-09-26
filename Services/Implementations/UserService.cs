using RideHailingAPI.DTOs.User;
using RideHailingAPI.Repositories.Interfaces;
using RideHailingAPI.Services.Interfaces;

namespace RideHailingAPI.Services.Implementations;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<UserResponse?> GetByIdAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
            return null;

        return new UserResponse
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

    public async Task<List<UserResponse>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();

        return users.Select(user => new UserResponse
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
        }).ToList();
    }

    public async Task<UserResponse?> UpdateProfileAsync(int userId, UpdateUserRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
            return null;
        
        var existingUser = await _userRepository.GetByPhoneNumberAsync(
            request.PhoneNumber ?? "");

        if (existingUser != null && existingUser.Id != userId)
        {
            throw new Exception("Phone number is already in use.");
        }

        user.FullName = request.FullName;
        user.PhoneNumber = request.PhoneNumber;
        
        user.IsPhoneVerified = false;

        await _userRepository.UpdateAsync(user);

        return new UserResponse
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
}