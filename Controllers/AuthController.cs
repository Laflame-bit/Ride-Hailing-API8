using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RideHailingAPI.DTOs.Auth;
using RideHailingAPI.Services.Interfaces;

namespace RideHailingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterRequest request)
    {
        try
        {
            var result = await _authService.RegisterAsync(request);

            return Ok(new
            {
                message = result
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginRequest request)
    {
        try
        {
            var result = await _authService.LoginAsync(request);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return Unauthorized(new
            {
                message = ex.Message
            });
        }
    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail(
        VerifyEmailOtpRequest request)
    {
        var result =
            await _authService.VerifyEmailOtpAsync(request);

        if (!result)
        {
            return BadRequest(new
            {
                message = "Invalid or expired OTP."
            });
        }

        return Ok(new
        {
            message = "Email verified successfully."
        });
    }

    [HttpPost("verify-phone")]
    public async Task<IActionResult> VerifyPhone(
        VerifyPhoneOtpRequest request)
    {
        var result =
            await _authService.VerifyPhoneOtpAsync(request);

        if (!result)
        {
            return BadRequest(new
            {
                message = "Invalid or expired OTP."
            });
        }

        return Ok(new
        {
            message = "Phone verified successfully."
        });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(
        ForgotPasswordRequest request)
    {
        await _authService.ForgotPasswordAsync(request);

        return Ok(new
        {
            message =
                "If the email exists, a password reset OTP has been sent."
        });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(
        ResetPasswordRequest request)
    {
        var result =
            await _authService.ResetPasswordAsync(request);

        if (!result)
        {
            return BadRequest(new
            {
                message = "Invalid or expired OTP."
            });
        }

        return Ok(new
        {
            message = "Password reset successfully."
        });
    }
    
    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(
        ChangePasswordRequest request)
    {
        var userIdClaim =
            User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
            return Unauthorized();

        var userId = int.Parse(userIdClaim.Value);

        var result =
            await _authService.ChangePasswordAsync(
                userId,
                request);

        if (!result)
        {
            return BadRequest(new
            {
                message = "Current password is incorrect."
            });
        }

        return Ok(new
        {
            message = "Password changed successfully."
        });
    }
    [HttpPost("resend-email-otp")]
    public async Task<IActionResult> ResendEmailOtp([FromBody] VerifyEmailOtpRequest request)
    {
        var result = await _authService.ResendEmailOtpAsync(request.Email ?? "");

        return Ok(new
        {
            message = "If the account exists, a new email OTP has been sent."
        });
    }

    [HttpPost("resend-phone-otp")]
    public async Task<IActionResult> ResendPhoneOtp([FromBody] VerifyPhoneOtpRequest request)
    {
        var result = await _authService.ResendPhoneOtpAsync(request.PhoneNumber ?? "");

        return Ok(new
        {
            message = "If the account exists, a new phone OTP has been sent."
        });
    }
}