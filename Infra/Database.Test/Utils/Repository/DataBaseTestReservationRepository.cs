using BusReservationService.Database.Interface;
using BusReservationService.Database.Repository;
using BusReservationService.Domain.Entity;
using BusReservationService.Database.Table;
using Dapper;

namespace BusReservationService.Infra.Database.Test.Utils.Repository;

/// <summary>
/// テストケースで使用するヘルパーリポジトリ
/// テストケースのための追加、更新、全件取得、削除を行うリポジトリ
/// </summary>
/// <param name="context">接続セッション</param>
internal class DataBaseTestReservationRepository(IDbContext context)
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
            sql, reservation, context.Transaction
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
            sql, reservation, context.Transaction
        );

        return table.ToDomain();
    }

    public async Task<ReservationEntity[]> GetAllAsync()
    {
        context.EnsureConnectionOpen();

        var sql = """
            SELECT * FROM reservations;
        """;

        var tables = await context.Connection.QueryAsync<ReservationTable>(sql, context.Transaction);

        if(tables.FirstOrDefault() is null)
        {
            return [];
        }

        return tables.Select(t => t.ToDomain()).ToArray();
    }

    public async Task<int> DeleteByAccountIdAsync(Guid accountId)
    {
        context.EnsureConnectionOpen();

        var sql = """
        DELETE FROM reservations WHERE account_id = @AccountId
        """;

        var ret = await context.Connection.ExecuteAsync(sql, new { AccountId = accountId }, context.Transaction);

        
        return ret;
    }

    public async Task<int> DeleteByParkingLotIdAsync(Guid parkingLotId)
    {
        context.EnsureConnectionOpen();

        var sql = """
        DELETE FROM reservations WHERE parking_lot_id = @ParkingLotId
        """;

        var ret = await context.Connection.ExecuteAsync(sql, new { ParkingLotId = parkingLotId }, context.Transaction);

        return ret;
    }

    public async Task<bool> DeleteByReservationIdAsync(Guid reservationId)
    {
        context.EnsureConnectionOpen();

        var sql = """
        DELETE FROM reservations WHERE id = @ReservationId
        """;

        var ret = await context.Connection.ExecuteAsync(sql, new { ReservationId = reservationId }, context.Transaction);

        return ret == 1;
    }
}