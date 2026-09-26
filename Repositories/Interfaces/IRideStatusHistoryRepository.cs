using RideHailingAPI.Domain.Entities;

namespace RideHailingAPI.Repositories.Interfaces;

public interface IRideStatusHistoryRepository
{
    Task<List<RideStatusHistory>> GetByRideIdAsync(int rideId);
    Task<RideStatusHistory> AddAsync(RideStatusHistory history);
}