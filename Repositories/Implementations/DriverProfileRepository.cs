using Microsoft.EntityFrameworkCore;
using RideHailingAPI.Data;
using RideHailingAPI.Domain.Entities;
using RideHailingAPI.Repositories.Interfaces;

namespace RideHailingAPI.Repositories.Implementations;

public class DriverProfileRepository  : IDriverProfileRepository
{
    private readonly AppDbContext _context;

    public DriverProfileRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<DriverProfile?> GetByIdAsync(int id)
    {
        return await _context.DriverProfiles
            .Include(d => d.User)
            .Include(d => d.Vehicle)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<DriverProfile?> GetByUserIdAsync(int userId)
    {
        return await _context.DriverProfiles
            .Include(d => d.User)
            .Include(d => d.Vehicle)
            .FirstOrDefaultAsync(d => d.UserId == userId);
    }

    public async Task<List<DriverProfile>> GetAllAsync()
    {
        return await _context.DriverProfiles
            .Include(d => d.User)
            .Include(d => d.Vehicle)
            .ToListAsync();
    }

    public async Task<DriverProfile> AddAsync(DriverProfile driverProfile)
    {
        await _context.DriverProfiles.AddAsync(driverProfile);
        await _context.SaveChangesAsync();

        return driverProfile;
    }

    public async Task UpdateAsync(DriverProfile driverProfile)
    {
        _context.DriverProfiles.Update(driverProfile);
        await _context.SaveChangesAsync();
    }
}