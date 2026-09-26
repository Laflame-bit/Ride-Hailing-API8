using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using RideHailingAPI.Domain.Entities;
using RideHailingAPI.Domain.Enums;
using RideHailingAPI.DTOs.Auth;
using RideHailingAPI.Repositories.Interfaces;
using RideHailingAPI.Services.Interfaces;

namespace RideHailingAPI.Services.Implementations;

public class AuthService  : IAuthService
{
    private readonly IOtpNotificationService _otpNotificationService;
    private readonly IUserRepository _userRepository;
    private readonly IEmailOtpRepository _emailOtpRepository;
    private readonly IPhoneOtpRepository _phoneOtpRepository;
    private readonly IPasswordResetOtpRepository _passwordResetOtpRepository;
    private readonly IConfiguration _configuration;

    public AuthService(
        IUserRepository userRepository,
        IEmailOtpRepository emailOtpRepository,
        IPhoneOtpRepository phoneOtpRepository,
        IPasswordResetOtpRepository passwordResetOtpRepository,
        IConfiguration configuration,
        IOtpNotificationService otpNotificationService)
    {
        _userRepository = userRepository;
        _emailOtpRepository = emailOtpRepository;
        _phoneOtpRepository = phoneOtpRepository;
        _passwordResetOtpRepository = passwordResetOtpRepository;
        _configuration = configuration;
        _otpNotificationService = otpNotificationService;
    }
    
    public async Task<string> RegisterAsync(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            throw new Exception("Email is required.");

        if (string.IsNullOrWhiteSpace(request.PhoneNumber))
            throw new Exception("Phone number is required.");

        if (string.IsNullOrWhiteSpace(request.Password))
            throw new Exception("Password is required.");

        var existingEmail = await _userRepository.GetByEmailAsync(request.Email);

        if (existingEmail != null)
            throw new Exception("Email already exists.");

        var existingPhone = await _userRepository.GetByPhoneNumberAsync(request.PhoneNumber);

        if (existingPhone != null)
            throw new Exception("Phone number already exists.");

        if (!Enum.TryParse<UserRole>(request.Role, true, out var role))
        {
            throw new Exception("Invalid role.");
        }

        if (role == UserRole.Admin)
        {
            throw new Exception("Admin registration is not allowed.");
        }

        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = role,
            IsEmailVerified = false,
            IsPhoneVerified = false,
            IsActive = true
        };

        await _userRepository.AddAsync(user);

        var emailOtp = new EmailOtp
        {
            UserId = user.Id,
            OtpCode = GenerateOtp(),
            ExpiresAt = DateTime.UtcNow.AddMinutes(5),
            IsUsed = false
        };

        await _emailOtpRepository.AddAsync(emailOtp);

        var phoneOtp = new PhoneOtp
        {
            UserId = user.Id,
            OtpCode = GenerateOtp(),
            ExpiresAt = DateTime.UtcNow.AddMinutes(5),
            IsUsed = false
        };

        await _phoneOtpRepository.AddAsync(phoneOtp);
        _otpNotificationService.SendEmailOtp(
            user.Email ?? "",
            emailOtp.OtpCode ?? "");

        _otpNotificationService.SendPhoneOtp(
            user.PhoneNumber ?? "",
            phoneOtp.OtpCode ?? "");

        return "Registration successful. Please verify your email and phone.";
    }
    

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email ?? "");

        if (user == null)
            throw new Exception("Invalid email or password.");

        if (!BCrypt.Net.BCrypt.Verify(
                request.Password,
                user.PasswordHash))
        {
            throw new Exception("Invalid email or password.");
        }

        if (!user.IsActive)
            throw new Exception("Account is inactive.");

        if (!user.IsEmailVerified || !user.IsPhoneVerified)
            throw new Exception("Please verify your email and phone before logging in.");

        var token = GenerateToken(user);

        return new LoginResponse
        {
            Token = token,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role.ToString()
        };
    }

    public async Task<bool> VerifyEmailOtpAsync(VerifyEmailOtpRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email ?? "");

        if (user == null)
            return false;

        var otp = await _emailOtpRepository.GetValidOtpAsync(
            user.Id,
            request.OtpCode ?? "");

        if (otp == null)
            return false;

        otp.IsUsed = true;
        user.IsEmailVerified = true;

        await _emailOtpRepository.UpdateAsync(otp);
        await _userRepository.UpdateAsync(user);

        return true;
    }

    public async Task<bool> VerifyPhoneOtpAsync(VerifyPhoneOtpRequest request)
    {
        var user = await _userRepository.GetByPhoneNumberAsync(
            request.PhoneNumber ?? "");

        if (user == null)
            return false;

        var otp = await _phoneOtpRepository.GetValidOtpAsync(
            user.Id,
            request.OtpCode ?? "");

        if (otp == null)
            return false;

        otp.IsUsed = true;
        user.IsPhoneVerified = true;

        await _phoneOtpRepository.UpdateAsync(otp);
        await _userRepository.UpdateAsync(user);

        return true;
    }

    public async Task<bool> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(
            request.Email ?? "");

        // Do not reveal whether the email exists.
        if (user == null)
            return true;
        await _passwordResetOtpRepository.InvalidateAllAsync(user.Id);
        var otp = new PasswordResetOtp
        {
            UserId = user.Id,
            OtpCode = GenerateOtp(),
            ExpiresAt = DateTime.UtcNow.AddMinutes(5),
            IsUsed = false
        };

        await _passwordResetOtpRepository.AddAsync(otp);
        
        _otpNotificationService.SendPasswordResetOtp(
            user.Email ?? "",
            otp.OtpCode ?? "");

        return true;
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(
            request.Email ?? "");

        if (user == null)
            return false;

        var otp = await _passwordResetOtpRepository.GetValidOtpAsync(
            user.Id,
            request.OtpCode ?? "");

        if (otp == null)
            return false;

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(
            request.NewPassword);

        otp.IsUsed = true;

        await _passwordResetOtpRepository.UpdateAsync(otp);
        await _userRepository.UpdateAsync(user);

        return true;
    }

    public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
            return false;

        if (!BCrypt.Net.BCrypt.Verify(
                request.CurrentPassword,
                user.PasswordHash))
        {
            return false;
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(
            request.NewPassword);

        await _userRepository.UpdateAsync(user);

        return true;
    }

    public async Task<bool> ResendEmailOtpAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);

        if (user == null)
            return false;

        await _emailOtpRepository.InvalidateAllAsync(user.Id);

        var otp = new EmailOtp
        {
            UserId = user.Id,
            OtpCode = GenerateOtp(),
            ExpiresAt = DateTime.UtcNow.AddMinutes(5),
            IsUsed = false
        };

        await _emailOtpRepository.AddAsync(otp);

        _otpNotificationService.SendEmailOtp(
            user.Email ?? "",
            otp.OtpCode ?? "");

        return true;
    }

    public async Task<bool> ResendPhoneOtpAsync(string phoneNumber)
    {
        var user = await _userRepository.GetByPhoneNumberAsync(phoneNumber);

        if (user == null)
            return false;

        await _phoneOtpRepository.InvalidateAllAsync(user.Id);

        var otp = new PhoneOtp
        {
            UserId = user.Id,
            OtpCode = GenerateOtp(),
            ExpiresAt = DateTime.UtcNow.AddMinutes(5),
            IsUsed = false
        };

        await _phoneOtpRepository.AddAsync(otp);

        _otpNotificationService.SendPhoneOtp(
            user.PhoneNumber ?? "",
            otp.OtpCode ?? "");

        return true;
    }

    private string GenerateOtp()
    {
        return Random.Shared
            .Next(100000, 1000000)
            .ToString();
    }

    private string GenerateToken(User user)
    {
        var jwtKey = _configuration["Jwt:Key"];

        if (string.IsNullOrWhiteSpace(jwtKey))
            throw new Exception("JWT key is not configured.");

        var claims = new[]
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                user.Id.ToString()),

            new Claim(
                ClaimTypes.Name,
                user.FullName ?? ""),

            new Claim(
                ClaimTypes.Email,
                user.Email ?? ""),

            new Claim(
                ClaimTypes.Role,
                user.Role.ToString()),

            new Claim(
                "phoneNumber",
                user.PhoneNumber ?? "")
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    
    }
   
}