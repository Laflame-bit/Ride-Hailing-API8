using RideHailingAPI.DTOs.Auth;

namespace RideHailingAPI.Services.Interfaces;

public interface IAuthService
{
    Task<string> RegisterAsync(RegisterRequest request);
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<bool> VerifyEmailOtpAsync(VerifyEmailOtpRequest request);
    Task<bool> VerifyPhoneOtpAsync(VerifyPhoneOtpRequest request);
    Task<bool> ForgotPasswordAsync(ForgotPasswordRequest request);
    Task<bool> ResetPasswordAsync(ResetPasswordRequest request);
    Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest request);
    Task<bool> ResendEmailOtpAsync(string email);
    Task<bool> ResendPhoneOtpAsync(string phoneNumber);
}