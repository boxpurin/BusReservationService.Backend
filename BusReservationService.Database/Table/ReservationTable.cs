
using BusReservationService.Database.Enum;

namespace BusReservationService.Database.Table;

public class ReservationTable

{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public Guid ParkingLotId { get; set; }
    public DateTime IncomingDateTime { get; set; }
    public DateTime OutgoingDateTime { get; set; }
    public ReservationStatus Status { get; set; }
    public DateTime? CanceledAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
