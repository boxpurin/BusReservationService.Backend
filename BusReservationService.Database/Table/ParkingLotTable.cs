namespace BusReservationService.Database.Table;

public class ParkingLotTable
{
    public Guid Id { get; set; }
    public required string ParkingLotName { get; set; }
    public short PrefectureId { get; set; }
    public required string StreetAddress { get; set; }
    public required string PhoneNumber { get; set; }
    public short MaxCapacity { get; set; }
    public short ReservationCapacity { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}