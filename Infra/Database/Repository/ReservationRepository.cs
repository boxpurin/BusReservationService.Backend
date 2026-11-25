using BusReservationService.Database.Interface;
using BusReservationService.Database.Table;
using BusReservationService.Domain.Entity;
using BusReservationService.Domain.Repository;
using BusReservationService.Share;
using Dapper;
using System.Data;

namespace BusReservationService.Database.Repository;

public class ReservationRepository(IDbContext context) : IDbContext, IReservationRepository
{
    public async Task<ReservationEntity> AddAsync(ReservationEntity reservation)
    {
        context.EnsureConnectionOpen();

        var sql = """
            INSERT INTO reservations ( 
            id, reservation_number, account_id, parking_lot_id,
            incoming_datetime, outgoing_datetime, 
            reservation_status, bus_campany_name, canceled_at
            ) VALUES 
            (@Id, @ReservationNumber, @AccountId, @ParkingLotId, 
            @IncomingDateTime, @OutGoingDateTime, 
            @ReservationStatus, @BusCampanyName, 
            @CanceledAt)
            RETURNING * ;
        """;

        var table = await context.Connection.QuerySingleAsync<ReservationTable>(
            sql, reservation
        );

        return table.ToDomain();
    }
    public async Task<ReservationEntity> UpdateAsync(ReservationEntity reservation)
    {
        context.EnsureConnectionOpen();

        var sql = """
            UPDATE reservations 
            SET 
            id = @Id,
            account_id = @AccountId, 
            parking_lot_id = @ParkingLotId, 
            incoming_datetime = @InComingDateTime, 
            outgoing_datetime = @OutGoingDateTime, 
            reservation_status = @ReservationStatus,
            bus_campany_name = @BusCampanyName,
            canceled_at = @CanceledAt 
            WHERE id = @Id 
            RETURNING * ;
        """;

        var table = await context.Connection.QuerySingleAsync<ReservationTable>(
            sql, reservation
        );

        return table.ToDomain();
    }

    public async Task<ReservationEntity[]> FindByAccountIdAsync(Guid accountId)
    {
        context.EnsureConnectionOpen();

        var sql = """
        SELECT * FROM reservations WHERE account_id = @AccountId
        """;

        var rows = await context.Connection.QueryAsync<ReservationTable>(
            sql, new { AccountId = accountId }
        );

        return rows.Select(r => r.ToDomain()).ToArray();
    }

    public async Task<ReservationEntity[]> FindByParkingLotIdAsync(Guid parkingLotId)
    {
        context.EnsureConnectionOpen();

        var sql = """
        SELECT * FROM reservations WHERE parking_lot_id = @ParkingLotId
        """;

        var rows = await context.Connection.QueryAsync<ReservationTable>(
            sql, new { ParkingLotId = parkingLotId }
        );

        return rows.Select(r => r.ToDomain()).ToArray();
    }

    public async Task<ReservationEntity?> FindByReservationIdAsync(Guid reservationId)
    {
        context.EnsureConnectionOpen();

        var sql = """
        SELECT * FROM reservations WHERE id = @ReservationId
        """;

        var row = await context.Connection.QuerySingleOrDefaultAsync<ReservationTable>(
            sql, new { ReservationId = reservationId }
        );

        return row?.ToDomain();
    }

    
    public async Task<ReservationEntity?> FindByReservationIdForUpdateAsync(Guid reservationId)
    {
        context.EnsureConnectionOpen();

        var sql = """
        SELECT * FROM reservations WHERE id = @ReservationId FOR UPDATE;
        """;

        var row = await context.Connection.QuerySingleOrDefaultAsync<ReservationTable>(
            sql, new { ReservationId = reservationId }
        );

        return row?.ToDomain();
    }

    public async Task<ReservationEntity[]> FindOverlappingByParkingLotIdAsync(Guid parkingLotId, DateTime from, DateTime to)
    {
        context.EnsureConnectionOpen();

        var sql = """
        SELECT * FROM reservations WHERE parking_lot_id = @ParkingLotId
        AND reservation_status != 'canceled'
        AND incoming_datetime < @To
        AND outgoing_datetime > @From;
        """;

        var rows = await context.Connection.QueryAsync<ReservationTable>(
            sql, new { ParkingLotId = parkingLotId, From = from, To = to }
        );

        return rows.Select(x => x.ToDomain()).ToArray();
    }

    public async Task<bool> ExistByReservationIdAsync(Guid reservationId)
    {
        context.EnsureConnectionOpen();

        var sql = """
        SELECT * FROM reservations WHERE id = @ReservationId
        """;

        var row = await context.Connection.QuerySingleOrDefaultAsync<ReservationTable>(
            sql, new { ReservationId = reservationId }
        );

        return row is not null;
    }
    public async Task<int> DeleteByAccountIdAsync(Guid accountId)
    {
        context.EnsureConnectionOpen();

        var sql = """
        DELETE FROM reservations WHERE account_id = @id
        """;

        var ret = await context.Connection.ExecuteAsync(sql, new { id = accountId });

        
        return ret;
    }

    public async Task<int> DeleteByParkingLotIdAsync(Guid parkingLotId)
    {
        context.EnsureConnectionOpen();

        var sql = """
        DELETE FROM reservations WHERE parking_lot_id = @id
        """;

        var tx = context.Connection.BeginTransaction();

        var ret = await context.Connection.ExecuteAsync(sql, new { id = parkingLotId }, tx);

        tx.Commit();

        return ret;
    }

    public async Task<bool> DeleteByReservationIdAsync(Guid reservationId)
    {
        context.EnsureConnectionOpen();

        var sql = """
        DELETE FROM reservations WHERE id = @id
        """;

        var ret = await context.Connection.ExecuteAsync(sql, new { id = reservationId });

        return ret == 1;
    }
}
