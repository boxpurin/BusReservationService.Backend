using BusReservationService.Application.Core.Models;
using BusReservationService.Share;
using BusReservationService.Share.Utils.Error;

namespace BusReservationService.Application.Core.Services.Interface;

public interface IParkingLotAvailabilityListService
{
    Task<ApplicationResult<ParkingLotAvailability, Error>> GetDailyAvailabilityAsync(Guid parkingLotId, DateOnly date);
}

public sealed class ParkingLotAvailabilityOptions
{
    public TimeSpan SlotInterval { get; set; } = TimeSpan.FromMinutes(15);
}