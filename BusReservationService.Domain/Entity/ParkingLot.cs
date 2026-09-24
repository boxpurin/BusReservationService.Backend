using System.ComponentModel.DataAnnotations.Schema;

namespace BusReservationService.Domain.Entity;

public class ParkingLotEntity
{
    public Guid Id { get; set; }
    public required string ParkingLotName { get; set; }
    public short PrefectureId { get; set; }
    public required string StreetAddress { get; set; }
    public required string PhoneNumber { get; set; }
    public short MaxCapacity { get; set; }
    public short ReservationCapacity { get; set; }
}
