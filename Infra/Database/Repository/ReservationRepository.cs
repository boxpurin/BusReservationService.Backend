using BusReservationService.Database.Interface;
using BusReservationService.Database.Table;
using BusReservationService.Domain.Entity;
using BusReservationService.Domain.Repository;
using BusReservationService.Share;
using Dapper;
using System.Data;

namespace BusReservationService.Database.Repository;

public class ReservationRepository(IDbConnection connection) : IConnection, IReservationRepository
{
    public async Task<ReservationEntity> AddAsync(ReservationEntity reservation)
    {
        EnsureConnectionOpen(connection);

        var sql = """
            INSERT INTO reservations ( 
            id, account_id, parking_lot_id,
            incoming_datetime, outgoing_datetime, 
            reservation_status, bus_campany_name, canceled_at
            ) VALUES 
            (@Id, @AccountId, @ParkingLotId, 
            @IncomingDateTime, @OutGoingDateTime, 
            @ReservationStatus, @BusCampanyName, 
            @CanceledAt) 
            RETURNING * ;
        """;

        var tx = connection.BeginTransaction();

        var table = await connection.QuerySingleAsync<ReservationTable>(
            sql, reservation, tx
        );

        tx.Commit();

        return table.ToDomain();
    }
    public async Task<ReservationEntity> UpdateAsync(ReservationEntity reservation)
    {
        EnsureConnectionOpen(connection);

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

        var tx = connection.BeginTransaction();

        var table = await connection.QuerySingleAsync<ReservationTable>(
            sql, reservation, tx
        );

        tx.Commit();

        return table.ToDomain();
    }

    public async Task<ReservationEntity[]> FindByAccountIdAsync(Guid accountId)
    {
        EnsureConnectionOpen(connection);

        var sql = """
        SELECT * FROM reservations WHERE account_id = @AccountId
        """;

        var rows = await connection.QueryAsync<ReservationTable>(
            sql, new { AccountId = accountId }
        );

        return rows.Select(r => r.ToDomain()).ToArray();
    }

    public async Task<ReservationEntity[]> FindByParkingLotIdAsync(Guid parkingLotId)
    {
        EnsureConnectionOpen(connection);

        var sql = """
        SELECT * FROM reservations WHERE parking_lot_id = @ParkingLotId
        """;

        var rows = await connection.QueryAsync<ReservationTable>(
            sql, new { ParkingLotId = parkingLotId }
        );

        return rows.Select(r => r.ToDomain()).ToArray();
    }

    public async Task<ReservationEntity?> FindByReservationIdAsync(Guid reservationId)
    {
        EnsureConnectionOpen(connection);

        var sql = """
        SELECT * FROM reservations WHERE id = @ReservationId
        """;

        var row = await connection.QuerySingleOrDefaultAsync<ReservationTable>(
            sql, new { ReservationId = reservationId }
        );

        return row?.ToDomain();
    }

    public async Task<bool> ExistByReservationIdAsync(Guid reservationId)
    {
        EnsureConnectionOpen(connection);

        var sql = """
        SELECT * FROM reservations WHERE id = @ReservationId
        """;

        var row = await connection.QuerySingleOrDefaultAsync<ReservationTable>(
            sql, new { ReservationId = reservationId }
        );

        return row is not null;
    }
    public async Task<int> DeleteByAccountIdAsync(Guid accountId)
    {
        EnsureConnectionOpen(connection);

        var sql = """
        DELETE FROM reservations WHERE account_id = @Id
        """;

        var tx = connection.BeginTransaction();

        var ret = await connection.ExecuteAsync(sql, new { id = accountId }, tx);

        tx.Commit();
        
        return ret;
    }

    public async Task<int> DeleteByParkingLotIdAsync(Guid parkingLotId)
    {
        EnsureConnectionOpen(connection);

        var sql = """
        DELETE FROM reservations WHERE parking_lot_id = @Id
        """;

        var tx = connection.BeginTransaction();

        var ret = await connection.ExecuteAsync(sql, new { id = parkingLotId }, tx);

        tx.Commit();

        return ret;
    }

    public async Task<bool> DeleteByReservationIdAsync(Guid reservationId)
    {
        EnsureConnectionOpen(connection);

        var sql = """
        DELETE FROM reservations WHERE id = @Id
        """;

        var tx = connection.BeginTransaction();

        var ret = await connection.ExecuteAsync(sql, new { id = reservationId }, tx);

        tx.Commit();

        return ret == 1;
    }
}
