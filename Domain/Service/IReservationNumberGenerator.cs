using BusReservationService.Domain.Repository;
using BusReservationService.Share.Utils.Error;
using BusReservationService.Share;
using BusReservationService.Domain.ValueObject.Unit;

namespace BusReservationService.Domain.Service;

public interface IReservationNumberGenerator
{
    Task<ReservationNumberUnit> GenerateAsync(Guid parkingLotId, DateOnly date);
}