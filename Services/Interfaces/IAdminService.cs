using RideHailingAPI.DTOs.Admin;
using RideHailingAPI.DTOs.Driver;

namespace RideHailingAPI.Services.Interfaces;

public interface IAdminService
{
    Task<List<AdminUserResponse>> GetAllUsersAsync();

    Task<AdminUserResponse?> GetUserByIdAsync(
        int userId);

    Task<AdminUserResponse?> UpdateUserStatusAsync(
        int userId,
        UpdateUserStatusRequest request);

    Task<DriverProfileResponse?> ApproveDriverAsync(
        int driverProfileId,
        DriverApprovalRequest request);
}