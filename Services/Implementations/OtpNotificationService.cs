using RideHailingAPI.Services.Interfaces;

namespace RideHailingAPI.Services.Implementations;

public class OtpNotificationService : IOtpNotificationService
{
    public void SendEmailOtp(string email, string otpCode)
    {
        Console.WriteLine(
            $"Email OTP sent to {email}: {otpCode}");
    }

    public void SendPhoneOtp(string phoneNumber, string otpCode)
    {
        Console.WriteLine(
            $"Phone OTP sent to {phoneNumber}: {otpCode}");

    }

    public void SendPasswordResetOtp(string email, string otpCode)
    {
        Console.WriteLine(
            $"Password reset OTP sent to {email}: {otpCode}");
    }
}