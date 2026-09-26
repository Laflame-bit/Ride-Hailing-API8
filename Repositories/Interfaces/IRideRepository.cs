using RideHailingAPI.Domain.Entities;

namespace RideHailingAPI.Repositories.Interfaces;

public interface IRideRepository
{
    Task<Ride?> GetByIdAsync(int id);
    Task<Ride?> GetByReferenceAsync(string rideReference);
    Task<List<Ride>> GetAllAsync();
    Task<List<Ride>> GetByPassengerIdAsync(int passengerId);
    Task<List<Ride>> GetByDriverIdAsync(int driverId);
    Task<List<Ride>> GetAvailableRidesAsync();
    Task<Ride> AddAsync(Ride ride);
    Task UpdateAsync(Ride ride);
}