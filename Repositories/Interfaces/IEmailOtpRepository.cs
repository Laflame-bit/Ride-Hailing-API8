using RideHailingAPI.Domain.Entities;

namespace RideHailingAPI.Repositories.Interfaces;

public interface IEmailOtpRepository
{
    Task<EmailOtp?> GetValidOtpAsync(int userId, string otpCode);
    Task<EmailOtp?> GetLatestAsync(int userId);
    Task<EmailOtp> AddAsync(EmailOtp otp);
    Task UpdateAsync(EmailOtp otp);
    Task InvalidateAllAsync(int userId);
}