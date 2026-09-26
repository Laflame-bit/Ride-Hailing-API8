namespace RideHailingAPI.DTOs.Auth;

public class VerifyPhoneOtpRequest
{
    public string? PhoneNumber { get; set; }
    public string? OtpCode { get; set; }
}