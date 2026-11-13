using System.Data;
using BusReservationService.Database.Enums;
using BusReservationService.Database.Interface;
using BusReservationService.Database.Table;
using BusReservationService.Domain.Entity;
using BusReservationService.Domain.Repository;
using BusReservationService.Services.Interface;
using BusReservationService.Share;
using BusReservationService.Share.Utils.Error;
using Dapper;


namespace BusReservationService.Database.Service;

public class ReservationCommandService(
    IDbConnection conn,
    IReservationRepository reservationRepository) : IConnection , IReservationCommandService
{
    public async Task<ApplicationResult<ReservationEntity, Error>> CreateReservationAsync(
        Guid accountId,
        Guid parkingLotId,
        DateTime IncomingDate,
        DateTime OutgoingDate,
        string BusCompany
    )
    {
        EnsureConnectionOpen(conn);
        using var tx = conn.BeginTransaction(IsolationLevel.ReadCommitted);

        try
        {
            // lock the target parking lot for update to prevent concurrent reservations
            var parkingLot = await LockParkingLotAsync(parkingLotId, tx);
            var overlaps = await reservationRepository.FindOverlappingByParkingLotIdAsync(
                parkingLotId, IncomingDate, OutgoingDate
            );
            
            if (!CanReserve(parkingLot, overlaps, IncomingDate, OutgoingDate))
            {
                tx.Rollback();
                return new Err<ReservationEntity, Error>(
                    new Error {
                        Message = "Cannot reserve the parking lot.",
                        ErrorCode = "CANNOT_RESERVE",
                        StatusCode = 409 }
                );
            }

            var entity = ReservationEntity.CreateNewReservation(
                accountId,
                parkingLotId,
                IncomingDate,
                OutgoingDate,
                BusCompany
            );

            entity.Confirm();
            var reservation = await reservationRepository.AddAsync(entity);
            tx.Commit();

            return new Ok<ReservationEntity, Error>(reservation);
        }
        catch (Exception)
        {
            tx.Rollback();
            throw;
        }
    }

    public async Task<ApplicationResult<ReservationEntity, Error>> UpdateReservationAsync(
        Guid accountId,
        Guid reservationId,
        DateTime IncomingDate,
        DateTime OutgoingDate,
        string BusCompany
    )
    {
        EnsureConnectionOpen(conn);
        using var tx = conn.BeginTransaction(IsolationLevel.ReadCommitted);

        try
        {
            var reservation = await LockReservationAsync(reservationId, tx);
            if (reservation.AccountId != accountId)
            {
                tx.Rollback();
                return new Err<ReservationEntity, Error>(
                    new Error {
                    Message = "Unauthorized to update this reservation.",
                    ErrorCode = "UNAUTHORIZED",
                    StatusCode = 403 }
                );
                    
            }

            var parkingLot = await LockParkingLotAsync(reservation.ParkingLotId, tx);

            var overlaps = (await reservationRepository.FindOverlappingByParkingLotIdAsync(
                reservation.ParkingLotId, IncomingDate, OutgoingDate
            )).Where(r => r.Id != reservationId).ToArray();
            
            if (!CanReserve(parkingLot, overlaps, IncomingDate, OutgoingDate))
            {
                tx.Rollback();
                return new Err<ReservationEntity, Error>(
                    new Error {
                        Message = "Cannot reserve the parking lot.",
                        ErrorCode = "CANNOT_RESERVE",
                        StatusCode = 409
                    });
            }

            var entity = ReservationEntity.FromSnapshot(
                reservation.Id,
                accountId,
                reservation.ParkingLotId,
                IncomingDate,
                OutgoingDate,
                reservation.ReservationStatus.ToDomain(),
                BusCompany,
                reservation.CanceledAt
            );

            var updated = await reservationRepository.UpdateAsync(entity);
            tx.Commit();
    
            return new Ok<ReservationEntity, Error>(updated);
        }
        catch (Exception)
        {
            tx.Rollback();
            throw;
        }
    }

    public async Task<ApplicationResult<ReservationEntity, Error>> CancelReservationAsync(Guid accountId, Guid reservationId)
    {
        EnsureConnectionOpen(conn);
        using var tx = conn.BeginTransaction(IsolationLevel.ReadCommitted);

        try
        {
            var reservation = await LockReservationAsync(reservationId, tx);
            if (reservation is null || reservation.AccountId != accountId)
            {
                tx.Rollback();
                return new Err<ReservationEntity, Error>(
                    new Error { Message = "Unauthorized to cancel this reservation.", ErrorCode = "UNAUTHORIZED", StatusCode = 403 }
                );
            }

            var parkingLot = await LockParkingLotAsync(reservation.ParkingLotId, tx);

            var entity = ReservationEntity.FromSnapshot(
                reservation.Id,
                accountId,
                reservation.ParkingLotId,
                reservation.IncomingDateTime,
                reservation.OutgoingDateTime,
                ReservationStatus.Canceled.ToDomain(),
                reservation.BusCampanyName,
                DateTime.UtcNow
            );

            var updated = await reservationRepository.UpdateAsync(entity);
            tx.Commit();

            return new Ok<ReservationEntity, Error>(updated);
        }
        catch(Exception)
        {
            tx.Rollback();
            throw;
        }
    }

    private async Task<ReservationTable> InsertReservationAsync(
        Guid accountId,
        Guid parkingLotId,
        DateTime incomingDateTime,
        DateTime outgoingDateTime,
        string busCompany,
        IDbTransaction transaction
    )
    {
        var sql = @"
            INSERT INTO reservations (account_id, parking_lot_id, incoming_datetime, outgoing_datetime, bus_company)
            VALUES (@AccountId, @ParkingLotId, @IncomingDateTime, @OutgoingDateTime, @BusCompany)
            RETURNING *;
        ";

        var reservation = await conn.QuerySingleAsync<ReservationTable>(
            sql,
            new
            {
                AccountId = accountId,
                ParkingLotId = parkingLotId,
                IncomingDateTime = incomingDateTime,
                OutgoingDateTime = outgoingDateTime,
                BusCompany = busCompany
            },
            transaction
        );

        return reservation;
    }

    private bool CanReserve(ParkingLotTable parkingLot, ReservationEntity[] overlaps,DateTime from, DateTime to)
    {
        if( parkingLot.IsActive is not true )
        {
            return false;
        }

        while( from < to )
        {
            var overlappingReservations = overlaps.Where(r => 
                r.IncomingDateTime < to && r.OutgoingDateTime > from
            );

            if( overlappingReservations.Count() >= parkingLot.ReservationCapacity )
            {
                return false;
            }

            from = from.AddMinutes(15);
        }

        return true;
    }

    private async Task<ReservationTable> LockReservationAsync(Guid reservationId, IDbTransaction transaction)
    {
        var sql = "SELECT * FROM reservations WHERE id = @ReservationId FOR UPDATE";
        var reservation = await conn.QuerySingleOrDefaultAsync<ReservationTable>(sql, new { ReservationId = reservationId }, transaction);
        if (reservation == null)
        {
            throw new InvalidOperationException($"Reservation with ID {reservationId} not found.");
        }
        return reservation;
    }

    private async Task<ParkingLotTable> LockParkingLotAsync(Guid parkingLotId, IDbTransaction transaction)
    {
        var sql = "SELECT * FROM parking_lots WHERE id = @ParkingLotId FOR UPDATE";
        var parkingLot = await conn.QuerySingleOrDefaultAsync<ParkingLotTable>(sql, new { ParkingLotId = parkingLotId }, transaction);
        if (parkingLot == null)
        {
            throw new InvalidOperationException($"Parking lot with ID {parkingLotId} not found.");
        }
        return parkingLot;
    }
    
}