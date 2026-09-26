using Microsoft.EntityFrameworkCore;
using RideHailingAPI.Data;
using RideHailingAPI.Domain.Entities;
using RideHailingAPI.Repositories.Interfaces;

namespace RideHailingAPI.Repositories.Implementations;

public class RideStatusHistoryRepository : IRideStatusHistoryRepository
{
    private readonly AppDbContext _context;

    public RideStatusHistoryRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<RideStatusHistory>> GetByRideIdAsync(int rideId)
    {
        return await _context.RideStatusHistories
            .Where(h => h.RideId == rideId)
            .OrderBy(h => h.CreatedAt)
            .ToListAsync();
    }

    public async Task<RideStatusHistory> AddAsync(RideStatusHistory history)
    {
        await _context.RideStatusHistories.AddAsync(history);
        await _context.SaveChangesAsync();

        return history;
    }
}