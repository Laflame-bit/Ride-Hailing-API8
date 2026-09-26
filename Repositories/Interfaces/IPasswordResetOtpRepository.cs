using RideHailingAPI.Domain.Entities;

namespace RideHailingAPI.Repositories.Interfaces;

public interface IPasswordResetOtpRepository
{
    Task<PasswordResetOtp?> GetValidOtpAsync(int userId, string otpCode);
    Task<PasswordResetOtp?> GetLatestAsync(int userId);
    Task<PasswordResetOtp> AddAsync(PasswordResetOtp otp);
    Task UpdateAsync(PasswordResetOtp otp);
    Task InvalidateAllAsync(int userId);
}