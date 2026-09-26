using Microsoft.EntityFrameworkCore;
using RideHailingAPI.Data;
using RideHailingAPI.Domain.Entities;
using RideHailingAPI.Repositories.Interfaces;

namespace RideHailingAPI.Repositories.Implementations;

public class EmailOtpRepository  : IEmailOtpRepository
{
    private readonly AppDbContext _context;

    public EmailOtpRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<EmailOtp?> GetValidOtpAsync(int userId, string otpCode)
    {
        return await _context.EmailOtps
            .FirstOrDefaultAsync(o =>
                o.UserId == userId &&
                o.OtpCode == otpCode &&
                !o.IsUsed &&
                o.ExpiresAt > DateTime.UtcNow);
    }

    public async Task<EmailOtp?> GetLatestAsync(int userId)
    {
        return await _context.EmailOtps
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<EmailOtp> AddAsync(EmailOtp otp)
    {
        await _context.EmailOtps.AddAsync(otp);
        await _context.SaveChangesAsync();

        return otp;
    }

    public async Task UpdateAsync(EmailOtp otp)
    {
        _context.EmailOtps.Update(otp);
        await _context.SaveChangesAsync();
    }

    public async Task InvalidateAllAsync(int userId)
    {
        var otps = await _context.EmailOtps
            .Where(x => x.UserId == userId && !x.IsUsed)
            .ToListAsync();

        foreach (var otp in otps)
        {
            otp.IsUsed = true;
        }

        await _context.SaveChangesAsync();
    }
}