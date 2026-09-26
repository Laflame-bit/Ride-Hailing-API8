using Microsoft.EntityFrameworkCore;
using RideHailingAPI.Domain.Entities;
using RideHailingAPI.Domain.Enums;

namespace RideHailingAPI.Data;

public  static class DbSeeder
{
    public static async Task SeedAdminAsync(AppDbContext context)
    {
        var adminExists = await context.Users
            .AnyAsync(u => u.Role == UserRole.Admin);

        if (adminExists)
            return;

        var admin = new User
        {
            FullName = "Olaoye Muhammed",
            Email = "admin@ridehailing.com",
            PhoneNumber = "08000000000",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@888!"),
            Role = UserRole.Admin,
            IsEmailVerified = true,
            IsPhoneVerified = true,
            IsActive = true
        };

        await context.Users.AddAsync(admin);
        await context.SaveChangesAsync();
    }
}