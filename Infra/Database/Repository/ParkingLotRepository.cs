using BusReservationService.Domain.Entity;
using BusReservationService.Domain.Repository;
using BusReservationService.Share;
using System.Data;

namespace BusReservationService.Database.Repository;

public class ParkingLotRepository : IParkingLotRepository
{
    private readonly IDbConnection _connection;
    public ParkingLotRepository(IDbConnection dbConnection)
    {
        _connection = dbConnection;
    }
    public Task<ParkingLotEntity> AddAsync(ParkingLotEntity entity)
    {
        throw new NotImplementedException();
    }

    public Task<ParkingLotEntity[]> AddListAsync(ParkingLotEntity[] entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistByParkingLotIdAsync(Guid parkingLotId)
    {
        throw new NotImplementedException();
    }

    public Task<ParkingLotEntity> FindByParkingLotIdAsync(Guid parkingLotId)
    {
        throw new NotImplementedException();
    }

    public Task<ParkingLotEntity[]> FindByPrefactureIdAsync(Guid prefactureId)
    {
        throw new NotImplementedException();
    }

    public Task<ParkingLotEntity[]> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<ParkingLotEntity> UpdateAsync(ParkingLotEntity entity)
    {
        throw new NotImplementedException();
    }

    public Task<ParkingLotEntity[]> UpdateListAsync(ParkingLotEntity[] entity)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteAsync(Guid parkingLotId)
    {
        throw new NotImplementedException();
    }
}
