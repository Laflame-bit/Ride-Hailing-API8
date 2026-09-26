using RideHailingAPI.Domain.Entities;
using RideHailingAPI.Domain.Enums;
using RideHailingAPI.DTOs.Ride;
using RideHailingAPI.Repositories.Interfaces;
using RideHailingAPI.Services.Interfaces;

namespace RideHailingAPI.Services.Implementations;

public class RideService : IRideService
{
    private readonly IRideRepository _rideRepository;
    private readonly IRideStatusHistoryRepository _historyRepository;
    private readonly IDriverProfileRepository _driverProfileRepository;

    public RideService(
        IRideRepository rideRepository,
        IRideStatusHistoryRepository historyRepository,
        IDriverProfileRepository driverProfileRepository)
    {
        _rideRepository = rideRepository;
        _historyRepository = historyRepository;
        _driverProfileRepository = driverProfileRepository;
    }
    
    public async Task<RideResponse> RequestRideAsync(int passengerId, CreateRideRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.PickupLocation))
            throw new Exception("Pickup location is required.");

        if (string.IsNullOrWhiteSpace(request.Destination))
            throw new Exception("Destination is required.");

        var ride = new Ride
        {
            RideReference = GenerateRideReference(),
            PassengerId = passengerId,
            PickupLocation = request.PickupLocation,
            Destination = request.Destination,
            Status = RideStatus.Requested
        };

        await _rideRepository.AddAsync(ride);

        await _historyRepository.AddAsync(new RideStatusHistory
        {
            RideId = ride.Id,
            Status = RideStatus.Requested,
            ChangedByUserId = passengerId
        });

        return MapToResponse(ride);
    }

    public async Task<RideResponse?> GetByIdAsync(int rideId, int userId)
    {
        var ride = await _rideRepository.GetByIdAsync(rideId);

        if (ride == null)
            return null;

        if (ride.PassengerId != userId &&
            ride.DriverId != userId)
        {
            return null;
        }

        return MapToResponse(ride);
    }

    public async Task<List<RideResponse>> GetPassengerRidesAsync(int passengerId)
    {
        var rides =
            await _rideRepository.GetByPassengerIdAsync(passengerId);

        return rides
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<List<RideResponse>> GetDriverRidesAsync(int driverId)
    {
        var rides =
            await _rideRepository.GetByDriverIdAsync(driverId);

        return rides
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<List<RideResponse>> GetAvailableRidesAsync()
    {
        var rides =
            await _rideRepository.GetAvailableRidesAsync();

        return rides
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<RideResponse?> AcceptRideAsync(int rideId, int driverId)
    {
        var ride = await _rideRepository.GetByIdAsync(rideId);

        if (ride == null)
            return null;

        if (ride.Status != RideStatus.Requested)
            throw new Exception("Ride cannot be accepted.");

        if (ride.DriverId != null)
            throw new Exception("Ride has already been assigned.");

        var driverProfile =
            await _driverProfileRepository.GetByUserIdAsync(driverId);

        if (driverProfile == null)
            throw new Exception("Driver profile not found.");

        if (!driverProfile.IsApproved)
            throw new Exception("Driver has not been approved.");

        if (!driverProfile.IsAvailable)
            throw new Exception("Driver is not available.");

        ride.DriverId = driverId;
        ride.Status = RideStatus.Accepted;

        driverProfile.IsAvailable = false;

        await _rideRepository.UpdateAsync(ride);
        await _driverProfileRepository.UpdateAsync(driverProfile);

        await _historyRepository.AddAsync(new RideStatusHistory
        {
            RideId = ride.Id,
            Status = RideStatus.Accepted,
            ChangedByUserId = driverId
        });

        return MapToResponse(ride);
    }

    public async Task<RideResponse?> RejectRideAsync(int rideId, int driverId)
    {
        var ride = await _rideRepository.GetByIdAsync(rideId);

        if (ride == null)
            return null;

        if (ride.Status != RideStatus.Requested)
            return null;

        if (ride.DriverId != null)
            return null;

        var driverProfile =
            await _driverProfileRepository.GetByUserIdAsync(driverId);

        if (driverProfile == null)
            return null;

        if (!driverProfile.IsApproved)
            return null;

        return MapToResponse(ride);
    }

    public async Task<RideResponse?> UpdateStatusAsync(int rideId, int driverId, UpdateRideStatusRequest request)
    {
        var ride = await _rideRepository.GetByIdAsync(rideId);

        if (ride == null)
            return null;

        if (ride.DriverId != driverId)
            return null;

        if (!IsValidTransition(
                ride.Status,
                request.Status))
        {
            throw new Exception(
                $"Invalid ride status transition from {ride.Status} to {request.Status}.");
        }

        ride.Status = request.Status;

        await _rideRepository.UpdateAsync(ride);

        await _historyRepository.AddAsync(new RideStatusHistory
        {
            RideId = ride.Id,
            Status = request.Status,
            ChangedByUserId = driverId
        });

        if (request.Status == RideStatus.Completed)
        {
            var driverProfile =
                await _driverProfileRepository.GetByUserIdAsync(driverId);

            if (driverProfile != null)
            {
                driverProfile.IsAvailable = true;

                await _driverProfileRepository.UpdateAsync(
                    driverProfile);
            }
        }

        return MapToResponse(ride);
    }

    public async Task<RideResponse?> CancelRideAsync(int rideId, int passengerId, CancelRideRequest request)
    {
        var ride = await _rideRepository.GetByIdAsync(rideId);

        if (ride == null)
            return null;

        if (ride.PassengerId != passengerId)
            return null;

        if (ride.Status != RideStatus.Requested &&
            ride.Status != RideStatus.Accepted)
        {
            throw new Exception(
                "Ride cannot be cancelled at this stage.");
        }

        ride.Status = RideStatus.Cancelled;

        await _rideRepository.UpdateAsync(ride);

        await _historyRepository.AddAsync(new RideStatusHistory
        {
            RideId = ride.Id,
            Status = RideStatus.Cancelled,
            ChangedByUserId = passengerId
        });

        if (ride.DriverId != null)
        {
            var driverProfile =
                await _driverProfileRepository.GetByUserIdAsync(
                    ride.DriverId.Value);

            if (driverProfile != null)
            {
                driverProfile.IsAvailable = true;

                await _driverProfileRepository.UpdateAsync(
                    driverProfile);
            }
        }

        return MapToResponse(ride);
    }

    public async Task<List<RideStatusHistoryResponse>> GetStatusHistoryAsync(int rideId, int userId)
    {
        var ride = await _rideRepository.GetByIdAsync(rideId);

        if (ride == null)
            return new List<RideStatusHistoryResponse>();

        if (ride.PassengerId != userId &&
            ride.DriverId != userId)
        {
            return new List<RideStatusHistoryResponse>();
        }

        var history =
            await _historyRepository.GetByRideIdAsync(rideId);

        return history
            .Select(h => new RideStatusHistoryResponse
            {
                Id = h.Id,
                RideId = h.RideId,
                Status = h.Status,
                ChangedByUserId = h.ChangedByUserId,
                CreatedAt = h.CreatedAt
            })
            .ToList();
    }

    public async Task<List<RideResponse>> GetAllAsync()
    {
        var rides = await _rideRepository.GetAllAsync();

        return rides
            .Select(MapToResponse)
            .ToList();
    }

    private bool IsValidTransition(
        RideStatus currentStatus,
        RideStatus newStatus)
    {
        return currentStatus switch
        {
            RideStatus.Accepted =>
                newStatus == RideStatus.DriverArriving,

            RideStatus.DriverArriving =>
                newStatus == RideStatus.DriverArrived,

            RideStatus.DriverArrived =>
                newStatus == RideStatus.InProgress,

            RideStatus.InProgress =>
                newStatus == RideStatus.Completed,

            _ => false
        };
    }
    
    private string GenerateRideReference()
    {
        return $"RIDE-{Guid.NewGuid():N}"
            .ToUpper();
    }

    private RideResponse MapToResponse(Ride ride)
    {
        return new RideResponse
        {
            Id = ride.Id,
            RideReference = ride.RideReference,
            PassengerId = ride.PassengerId,
            DriverId = ride.DriverId,
            PickupLocation = ride.PickupLocation,
            Destination = ride.Destination,
            Status = ride.Status,
            CreatedAt = ride.CreatedAt
        };
    }
}