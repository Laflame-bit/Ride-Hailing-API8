namespace RideHailingAPI.DTOs.Auth;

public class VerifyEmailOtpRequest
{
    public string? Email { get; set; }
    public string? OtpCode { get; set; }
}