namespace BusReservationService.Domain.Entity;

public enum ReservationStatus
{
    Pending,
    Confirmed,
    Canceled,
}

public class ReservationEntity

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
