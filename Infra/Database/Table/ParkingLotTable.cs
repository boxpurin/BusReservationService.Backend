using BusReservationService.Domain.Entity;
using BusReservationService.Share;

namespace BusReservationService.Database.Table;

public class ParkingLotTable : IConvertibleToDomain<ParkingLotEntity>
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public short PrefectureId { get; set; }
    public string ZipCode { get; set; } = string.Empty;
    public string StreetAddress { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string FaxNumber { get; set; } = string.Empty;
    public short MaxCapacity { get; set; }
    public short ReservationCapacity { get; set; }
    public bool IsAllowsBoardingAlighting { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ParkingLotEntity ToDomain()
    {
        throw new NotImplementedException();
    }
}