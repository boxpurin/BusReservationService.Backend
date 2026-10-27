using BusReservationService.Domain.Entity;
using BusReservationService.Domain.ValueObject.Prefecture;
using BusReservationService.Share;

namespace BusReservationService.Domain.Repository;

public interface IParkingLotRepository
{

    Task<ParkingLotEntity> AddAsync(AddParkingLotDto dto);
    Task<ParkingLotEntity> UpdateAsync(ParkingLotEntity entity);
    Task<ParkingLotEntity[]> GetAllAsync();
    Task<ParkingLotEntity?> FindByParkingLotIdAsync(Guid parkingLotId);
    Task<ParkingLotEntity[]> FindByPrefactureIdAsync(Prefecture prefactureId);
    Task<bool> ExistByParkingLotIdAsync(Guid parkingLotId);
    Task<bool> ExistByPrefectureAsync(Prefecture prefecture);
    Task<bool> DeleteAsync(Guid parkingLotId);
}

public record AddParkingLotDto(
    string ParkingLotName,
    string ZipCode,
    Prefecture Prefecture,
    string StreetAddress,
    string PhoneNumber,
    string? FaxNumber,
    short MaxCapacity,
    short ReservationCapacity,
    bool IsAllowsBoardingAlighting = false,
    bool IsActive = false
);