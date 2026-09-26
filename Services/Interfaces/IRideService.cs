using RideHailingAPI.DTOs.Ride;

namespace RideHailingAPI.Services.Interfaces;

public interface IRideService
{
    Task<RideResponse> RequestRideAsync(
        int passengerId,
        CreateRideRequest request);

    Task<RideResponse?> GetByIdAsync(
        int rideId,
        int userId);

    Task<List<RideResponse>> GetPassengerRidesAsync(
        int passengerId);

    Task<List<RideResponse>> GetDriverRidesAsync(
        int driverId);

    Task<List<RideResponse>> GetAvailableRidesAsync();

    Task<RideResponse?> AcceptRideAsync(
        int rideId,
        int driverId);
    
    Task<RideResponse?> RejectRideAsync(int rideId, int driverId);

    Task<RideResponse?> UpdateStatusAsync(
        int rideId,
        int driverId,
        UpdateRideStatusRequest request);

    Task<RideResponse?> CancelRideAsync(
        int rideId,
        int passengerId,
        CancelRideRequest request);

    Task<List<RideStatusHistoryResponse>> GetStatusHistoryAsync(
        int rideId,
        int userId);

    Task<List<RideResponse>> GetAllAsync();
}