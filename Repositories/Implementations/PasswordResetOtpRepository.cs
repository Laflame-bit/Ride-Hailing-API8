using Microsoft.EntityFrameworkCore;
using RideHailingAPI.Data;
using RideHailingAPI.Domain.Entities;
using RideHailingAPI.Repositories.Interfaces;

namespace RideHailingAPI.Repositories.Implementations;

public class PasswordResetOtpRepository  : IPasswordResetOtpRepository
{
    private readonly AppDbContext _context;

    public PasswordResetOtpRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<PasswordResetOtp?> GetValidOtpAsync(int userId, string otpCode)
    {
        return await _context.PasswordResetOtps
            .FirstOrDefaultAsync(o =>
                o.UserId == userId &&
                o.OtpCode == otpCode &&
                !o.IsUsed &&
                o.ExpiresAt > DateTime.UtcNow);
    }

    public async Task<PasswordResetOtp?> GetLatestAsync(int userId)
    {
        return await _context.PasswordResetOtps
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<PasswordResetOtp> AddAsync(PasswordResetOtp otp)
    {
        await _context.PasswordResetOtps.AddAsync(otp);
        await _context.SaveChangesAsync();

        return otp;
    }

    public async Task UpdateAsync(PasswordResetOtp otp)
    {
        _context.PasswordResetOtps.Update(otp);
        await _context.SaveChangesAsync();
    }

    public async Task InvalidateAllAsync(int userId)
    {
        var otps = await _context.PasswordResetOtps
            .Where(x => x.UserId == userId && !x.IsUsed)
            .ToListAsync();

        foreach (var otp in otps)
        {
            otp.IsUsed = true;
        }

        await _context.SaveChangesAsync();
    }
}