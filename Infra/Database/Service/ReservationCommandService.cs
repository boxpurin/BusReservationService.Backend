using System.Data;
using BusReservationService.Database.Enums;
using BusReservationService.Database.Interface;
using BusReservationService.Database.Table;
using BusReservationService.Domain.Entity;
using BusReservationService.Domain.Repository;
using BusReservationService.Domain.Service;
using BusReservationService.Domain.ValueObject.Unit;
using BusReservationService.Infra.Database;
using BusReservationService.Services.Interface;
using BusReservationService.Share;
using BusReservationService.Share.Utils.Error;
using Dapper;


namespace BusReservationService.Database.Service;

public class ReservationCommandService(
    IUnitOfWork session,
    IReservationNumberGenerator reservationNumberGenerator,
    IReservationRepository reservationRepository,
    IParkingLotRepository parkingLotRepository) : IReservationCommandService
{
    public async Task<ApplicationResult<ReservationEntity, Error>> CreateReservationAsync(
        Guid accountId,
        Guid parkingLotId,
        DateTime IncomingDate,
        DateTime OutgoingDate,
        string BusCompany
    )
    {
        if (IncomingDate >= OutgoingDate)
        {
            return new Err<ReservationEntity, Error>(
                new Error {
                    Message = "Incoming date must be before outgoing date.",
                    ErrorCode = "INVALID_DATE_RANGE",
                    StatusCode = 400
                }
            );
        }

        await session.BeginAsync(IsolationLevel.ReadCommitted);

        try
        {
            // lock the target parking lot for update to prevent concurrent reservations
            var parkingLot = await parkingLotRepository.FindByParkingLotIdForUpdateAsync(parkingLotId);
            if (parkingLot == null)
            {
                await session.RollbackAsync();
                return new Err<ReservationEntity, Error>(
                    new Error {
                        Message = "Parking lot not found.",
                        ErrorCode = "PARKING_LOT_NOT_FOUND",
                        StatusCode = 404 }
                );
            }

            var overlaps = await reservationRepository.FindOverlappingByParkingLotIdAsync(
                parkingLotId, IncomingDate, OutgoingDate
            );
            
            if (!CanReserve(parkingLot, overlaps, IncomingDate, OutgoingDate))
            {
                await session.RollbackAsync();
                return new Err<ReservationEntity, Error>(
                    new Error {
                        Message = "Cannot reserve the parking lot.",
                        ErrorCode = "CANNOT_RESERVE",
                        StatusCode = 409 }
                );
            }

            var reservationNumber = await reservationNumberGenerator.GenerateAsync(
                parkingLotId, DateOnly.FromDateTime(IncomingDate)
            );

            var entity = ReservationEntity.CreateNewReservation(
                reservationNumber,
                accountId,
                parkingLotId,
                IncomingDate,
                OutgoingDate,
                BusCompany
            );

            entity.Confirm();
            var reservation = await reservationRepository.AddAsync(entity);
            await session.CommitAsync();

            return new Ok<ReservationEntity, Error>(reservation);
        }
        catch (Exception)
        {
            await session.RollbackAsync();
            throw;
        }
    }

    public async Task<ApplicationResult<ReservationEntity, Error>> UpdateReservationAsync(
        Guid accountId,
        Guid reservationId,
        Guid updatedToParkingLotId,
        DateTime IncomingDate,
        DateTime OutgoingDate,
        string BusCompany
    )
    {
        if (IncomingDate >= OutgoingDate)
        {
            return new Err<ReservationEntity, Error>(
                new Error {
                    Message = "Incoming date must be before outgoing date.",
                    ErrorCode = "INVALID_DATE_RANGE",
                    StatusCode = 400
                }
            );
        }
    
        await session.BeginAsync(IsolationLevel.ReadCommitted);

        try
        {
            var reservation = await reservationRepository.FindByReservationIdForUpdateAsync(reservationId);
            if (reservation == null)
            {
                await session.RollbackAsync();
                return new Err<ReservationEntity, Error>(
                    new Error { Message = "Reservation not found.", ErrorCode = "RESERVATION_NOT_FOUND", StatusCode = 404 }
                );
            }

            if (reservation.AccountId != accountId)
            {
                await session.RollbackAsync();
                return new Err<ReservationEntity, Error>(
                    new Error {
                    Message = "Unauthorized to update this reservation.",
                    ErrorCode = "UNAUTHORIZED",
                    StatusCode = 403 }
                );
                    
            }

            var parkingLotIds = new[] { reservation.ParkingLotId, updatedToParkingLotId }
                .Distinct()
                .OrderBy(id => id);

            var lockedParkingLots = new Dictionary<Guid, ParkingLotEntity>();

            foreach (var id in parkingLotIds)
            {
                var idLocked = await parkingLotRepository.FindByParkingLotIdForUpdateAsync(id);
                if (idLocked == null)
                {
                    await session.RollbackAsync();
                    return new Err<ReservationEntity, Error>(
                        new Error {
                            Message = "Parking lot not found.",
                            ErrorCode = "PARKING_LOT_NOT_FOUND",
                            StatusCode = 404 }
                    );
                }
                lockedParkingLots.Add(id, idLocked);
            }

            var targetParkingLot = lockedParkingLots[updatedToParkingLotId];

            var overlaps = (await reservationRepository.FindOverlappingByParkingLotIdAsync(
                targetParkingLot.Id, IncomingDate, OutgoingDate
            )).Where(r => r.Id != reservationId).ToArray();

            if (!CanReserve(targetParkingLot, overlaps, IncomingDate, OutgoingDate))
            {
                await session.RollbackAsync();
                return new Err<ReservationEntity, Error>(
                    new Error {
                        Message = "Cannot reserve the parking lot.",
                        ErrorCode = "CANNOT_RESERVE",
                        StatusCode = 409
                    });
            }

            var entity = ReservationEntity.FromSnapshot(
                reservation.Id,
                reservation.ReservationNumber,
                accountId,
                targetParkingLot.Id,
                IncomingDate,
                OutgoingDate,
                reservation.ReservationStatus,
                BusCompany,
                reservation.CanceledAt
            );

            var updated = await reservationRepository.UpdateAsync(entity);
            await session.CommitAsync();
    
            return new Ok<ReservationEntity, Error>(updated);
        }
        catch (Exception)
        {
            await session.RollbackAsync();
            throw;
        }
    }

    public async Task<ApplicationResult<ReservationEntity, Error>> CancelReservationAsync(Guid accountId, Guid reservationId)
    {
        await session.BeginAsync(IsolationLevel.ReadCommitted);

        try
        {
            var reservation = await reservationRepository.FindByReservationIdForUpdateAsync(reservationId);
            if (reservation is null)
            {
                await session.RollbackAsync();
                return new Err<ReservationEntity, Error>(
                    new Error { Message = "Reservation not found.", ErrorCode = "RESERVATION_NOT_FOUND", StatusCode = 404 }
                );
            }

            if (reservation.AccountId != accountId)
            {
                await session.RollbackAsync();
                return new Err<ReservationEntity, Error>(
                    new Error { Message = "Unauthorized to cancel this reservation.", ErrorCode = "UNAUTHORIZED", StatusCode = 403 }
                );
            }

            var parkingLot = await  parkingLotRepository.FindByParkingLotIdForUpdateAsync(reservation.ParkingLotId);
            if (parkingLot is null)
            {
                await session.RollbackAsync();
                return new Err<ReservationEntity, Error>(
                    new Error {
                        Message = "Parking lot not found.",
                        ErrorCode = "PARKING_LOT_NOT_FOUND",
                        StatusCode = 404 }
                );
            }

            var entity = ReservationEntity.FromSnapshot(
                reservation.Id,
                reservation.ReservationNumber,
                accountId,
                reservation.ParkingLotId,
                reservation.IncomingDateTime,
                reservation.OutgoingDateTime,
                ReservationStatus.Canceled.ToDomain(),
                reservation.BusCampanyName,
                DateTime.UtcNow
            );

            var updated = await reservationRepository.UpdateAsync(entity);
            await session.CommitAsync();

            return new Ok<ReservationEntity, Error>(updated);
        }
        catch(Exception)
        {
            await session.RollbackAsync();
            throw;
        }
    }

    private bool CanReserve(ParkingLotEntity parkingLot, ReservationEntity[] overlaps,DateTime from, DateTime to)
    {
        if( parkingLot.IsActive is not true )
        {
            return false;
        }

        var at = from;

        while( at < to )
        {
            if( overlaps.Count(r =>
                    r.IncomingDateTime <= at && at < r.OutgoingDateTime
                ) >= parkingLot.ReservationCapacity )
            {
                return false;
            }

            at = at.AddMinutes(15);
        }

        return true;
    }
}