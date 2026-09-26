using Microsoft.EntityFrameworkCore;
using RideHailingAPI.Data;
using RideHailingAPI.Domain.Entities;
using RideHailingAPI.Repositories.Interfaces;

namespace RideHailingAPI.Repositories.Implementations;

public class PhoneOtpRepository : IPhoneOtpRepository
{
    private readonly AppDbContext _context;

    public PhoneOtpRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<PhoneOtp?> GetValidOtpAsync(int userId, string otpCode)
    {
        return await _context.PhoneOtps
            .FirstOrDefaultAsync(o =>
                o.UserId == userId &&
                o.OtpCode == otpCode &&
                !o.IsUsed &&
                o.ExpiresAt > DateTime.UtcNow);
    }

    public async Task<PhoneOtp?> GetLatestAsync(int userId)
    {
        return await _context.PhoneOtps
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<PhoneOtp> AddAsync(PhoneOtp otp)
    {
        await _context.PhoneOtps.AddAsync(otp);
        await _context.SaveChangesAsync();

        return otp;
    }

    public async Task UpdateAsync(PhoneOtp otp)
    {
        _context.PhoneOtps.Update(otp);
        await _context.SaveChangesAsync();
    }

    public async Task InvalidateAllAsync(int userId)
    {
        var otps = await _context.PhoneOtps
            .Where(x => x.UserId == userId && !x.IsUsed)
            .ToListAsync();

        foreach (var otp in otps)
        {
            otp.IsUsed = true;
        }

        await _context.SaveChangesAsync();
    }
}