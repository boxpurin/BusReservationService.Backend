using BusReservationService.Database.Interface;
using BusReservationService.Domain.Service;
using BusReservationService.Domain.ValueObject.Unit;
using BusReservationService.Infra.Database;
using BusReservationService.Share;
using BusReservationService.Share.Utils.Error;
using Dapper;


public class ReservationNumberGenerator(IDbContext session) : IReservationNumberGenerator
{
    public async Task<ReservationNumberUnit>
    GenerateAsync(Guid parkingLotId, DateOnly date)
    {
        var seq = await session.Connection.ExecuteScalarAsync<long>("""
            SELECT COUNT(*) FROM reservations
            WHERE parking_lot_id = @ParkingLotId
            AND (incoming_datetime AT TIME ZONE 'Asia/Tokyo')::date = @Date
        """,
        new { ParkingLotId = parkingLotId, Date = date },
        session.Transaction);

        // 1日の予約件数として１万件超えることは物理的に起こり得ないものとして考える
        return new ReservationNumberUnit(
            $"{date.Day:D2}{seq + 1:D4}"
        );
    }
}
