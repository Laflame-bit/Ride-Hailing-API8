namespace RideHailingAPI.Services.Interfaces;

public interface IOtpNotificationService
{
    void SendEmailOtp(
        string email,
        string otpCode);

    void SendPhoneOtp(
        string phoneNumber,
        string otpCode);

    void SendPasswordResetOtp(
        string email,
        string otpCode);
}