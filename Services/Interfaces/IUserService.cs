using RideHailingAPI.DTOs.User;

namespace RideHailingAPI.Services.Interfaces;

public interface IUserService
{
    Task<UserResponse?> GetByIdAsync(int userId);
    Task<List<UserResponse>> GetAllAsync();
    Task<UserResponse?> UpdateProfileAsync(int userId, UpdateUserRequest request);
}