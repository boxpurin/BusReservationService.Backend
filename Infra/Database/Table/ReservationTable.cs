using BusReservationService.Database.Enums;
using BusReservationService.Domain.Entity;
using BusReservationService.Share;

namespace BusReservationService.Database.Table;

internal class ReservationTable : IConvertibleToDomain<ReservationEntity>
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public Guid ParkingLotId { get; set; }
    public DateTime IncomingDateTime { get; set; }
    public DateTime OutgoingDateTime { get; set; }
    public ReservationStatus ReservationStatus { get; set; }
    public string BusCampanyName {get; set;} = string.Empty;
    public DateTime? CanceledAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ReservationEntity ToDomain()
    {
        return ReservationEntity
        .FromSnapshot(
            Id,
            AccountId,
            ParkingLotId,
            IncomingDateTime,
            OutgoingDateTime,
            ReservationStatus.ToDomain(),
            BusCampanyName,
            CanceledAt
        );
    }
}
