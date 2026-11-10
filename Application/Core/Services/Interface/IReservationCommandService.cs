using BusReservationService.Domain.Entity;
using BusReservationService.Share;
using BusReservationService.Share.Utils.Error;

namespace BusReservationService.Services.Interface;

public interface IReservationCommandService
{
    Task<ApplicationResult<ReservationEntity, Error>> CreateReservationAsync(
        Guid accountId,
        Guid parkingLotId,
        DateTime IncomingDate,
        DateTime OutgoingDate,
        string BusCompany
    );

    Task<ApplicationResult<ReservationEntity, Error>> UpdateReservationAsync(
        Guid accountId,
        Guid reservationId,
        DateTime IncomingDate,
        DateTime OutgoingDate,
        string BusCompany
    );


    Task<ApplicationResult<ReservationEntity, Error>> CancelReservationAsync(
        Guid accountId,
        Guid reservationId
    );
}