using RideHailingAPI.Domain.Entities;

namespace RideHailingAPI.Repositories.Interfaces;

public interface IPhoneOtpRepository
{
    Task<PhoneOtp?> GetValidOtpAsync(int userId, string otpCode);
    Task<PhoneOtp?> GetLatestAsync(int userId);
    Task<PhoneOtp> AddAsync(PhoneOtp otp);
    Task UpdateAsync(PhoneOtp otp);
    Task InvalidateAllAsync(int userId);
}