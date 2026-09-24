using BusReservationService.Domain.Entity;
using BusReservationService.Share;

namespace BusReservationService.Domain.Repository;

public interface IParkingLotRepository
{

    Task<ParkingLotEntity> AddAsync(ParkingLotEntity entity);
    Task<ParkingLotEntity[]> AddListAsync(ParkingLotEntity[] entity);
    Task<ParkingLotEntity> UpdateAsync(ParkingLotEntity entity);
    Task<ParkingLotEntity[]> UpdateListAsync(ParkingLotEntity[] entity);
    Task<ParkingLotEntity[]> GetAllAsync();
    Task<ParkingLotEntity> FindByParkingLotIdAsync(Guid parkingLotId);
    Task<ParkingLotEntity[]> FindByPrefactureIdAsync(Guid prefactureId);
    Task<bool> ExistByParkingLotIdAsync(Guid parkingLotId);
    Task<bool> DeleteAsync(Guid parkingLotId);
}