using Microsoft.EntityFrameworkCore;
using RideHailingAPI.Data;
using RideHailingAPI.Domain.Entities;
using RideHailingAPI.Domain.Enums;
using RideHailingAPI.Repositories.Interfaces;

namespace RideHailingAPI.Repositories.Implementations;

public class RideRepository  : IRideRepository
{
    private readonly AppDbContext _context;

    public RideRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<Ride?> GetByIdAsync(int id)
    {
        return await _context.Rides
            .Include(r => r.Passenger)
            .Include(r => r.Driver)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Ride?> GetByReferenceAsync(string rideReference)
    {
        return await _context.Rides
            .Include(r => r.Passenger)
            .Include(r => r.Driver)
            .FirstOrDefaultAsync(r => r.RideReference == rideReference);
    }

    public async Task<List<Ride>> GetAllAsync()
    {
        return await _context.Rides
            .Include(r => r.Passenger)
            .Include(r => r.Driver)
            .ToListAsync();
    }

    public async Task<List<Ride>> GetByPassengerIdAsync(int passengerId)
    {
        return await _context.Rides
            .Where(r => r.PassengerId == passengerId)
            .ToListAsync();
    }

    public async Task<List<Ride>> GetByDriverIdAsync(int driverId)
    {
        return await _context.Rides
            .Where(r => r.DriverId == driverId)
            .ToListAsync();
    }

    public async Task<List<Ride>> GetAvailableRidesAsync()
    {
        return await _context.Rides
            .Where(r => r.Status == RideStatus.Requested && r.DriverId == null)
            .Include(r => r.Passenger)
            .ToListAsync();
    }

    public async Task<Ride> AddAsync(Ride ride)
    {
        await _context.Rides.AddAsync(ride);
        await _context.SaveChangesAsync();

        return ride;
    }

    public async Task UpdateAsync(Ride ride)
    {
        _context.Rides.Update(ride);
        await _context.SaveChangesAsync();
    }
}