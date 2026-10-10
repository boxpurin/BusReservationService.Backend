using BusReservationService.Database.Interface;
using BusReservationService.Domain.Entity;
using BusReservationService.Domain.Repository;
using BusReservationService.Share;
using System.Data;

namespace BusReservationService.Database.Repository;

public class ParkingLotRepository(IDbConnection connection) : IConnection, IParkingLotRepository
{
    public Task<ParkingLotEntity> AddAsync(ParkingLotEntity entity)
    {
        EnsureConnectionOpen(connection);

        throw new NotImplementedException();
    }

    public Task<bool> ExistByParkingLotIdAsync(Guid parkingLotId)
    {
        EnsureConnectionOpen(connection);

        throw new NotImplementedException();
    }

    public Task<ParkingLotEntity> FindByParkingLotIdAsync(Guid parkingLotId)
    {
        EnsureConnectionOpen(connection);

        throw new NotImplementedException();
    }

    public Task<ParkingLotEntity[]> FindByPrefactureIdAsync(Guid prefactureId)
    {
        EnsureConnectionOpen(connection);

        throw new NotImplementedException();
    }

    public Task<ParkingLotEntity[]> GetAllAsync()
    {
        EnsureConnectionOpen(connection);

        throw new NotImplementedException();
    }

    public Task<ParkingLotEntity> UpdateAsync(ParkingLotEntity entity)
    {
        EnsureConnectionOpen(connection);

        throw new NotImplementedException();
    }

    public async Task<bool> DeleteAsync(Guid parkingLotId)
    {
        EnsureConnectionOpen(connection);

        throw new NotImplementedException();
    }
}
