using System.Data;
using Dapper;
using BusReservationService.Database.Interface;
using BusReservationService.Database.Table;
using BusReservationService.Domain.Repository;
using BusReservationService.Share;
using BusReservationService.Share.Utils.Error;

public class ParkingLotCommandService(
    IDbConnection conn,
    IParkingLotRepository parkingLotRepository,
    IReservationRepository reservationRepository)
    : IConnection, IParkingLotCommandService
{
    public async Task<ApplicationResult<int, Error>> CountAvailableReservationSpace(Guid parkingLotId, DateTime datetime)
        {
            EnsureConnectionOpen(conn);

            try
            {
                var parkingLot = await parkingLotRepository.FindByParkingLotIdAsync(parkingLotId);

                if(parkingLot is null)
                {
                    return new Err<int, Error>(
                        new Error{
                            Message = "Parking lot does not exist",
                            ErrorCode = "PARKING_LOT_NOT_FOUND",
                            StatusCode = 404
                        }
                    );
                }
                var overlapping = await reservationRepository.FindOverlappingByParkingLotIdAsync(
                    parkingLotId, datetime.Date,
                    datetime.Date.AddDays(1));

                var available = parkingLot.ReservationCapacity - overlapping.Count(p =>
                {
                    return p.IncomingDateTime <= datetime && datetime < p.OutgoingDateTime ;
                });

                return new Ok<int, Error>(available);
            }
            catch
            {
                throw;
            }
        }

    public async Task<ApplicationResult<bool, Error>> HasAvailableReservationSpace(Guid parkingLotId, DateTime datetime)
    {
        var availableResult = await CountAvailableReservationSpace(parkingLotId, datetime);
        if (availableResult is Err<int, Error> (var va))
        {
            return new Err<bool, Error>(va);
        }

        var count = availableResult.Unwrap();
        return new Ok<bool, Error>(count > 0);
    }
}