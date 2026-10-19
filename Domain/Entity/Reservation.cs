using BusReservationService.Domain.ValueObject;

namespace BusReservationService.Domain.Entity;

public class ReservationEntity
{
    public Guid Id { get; private set; }
    public Guid AccountId { get; private set; }
    public Guid ParkingLotId { get; private set; }
    public DateTime IncomingDateTime { get; private set; }
    public DateTime OutgoingDateTime { get; private set; }
    public ReservationStatus Status { get; private set; }
    public string BusCampanyName { get; private set; }
    public DateTime? CanceledAt { get; private set; }

    private ReservationEntity(
        Guid id,
        Guid accountId,
        Guid parkingLotId,
        DateTime inComingDateTime,
        DateTime outGoingDateTime,
        ReservationStatus reservationStatus,
        string busCampanyName,
        DateTime? canceledAt
    )
    {
        Id = id;
        AccountId = accountId;
        ParkingLotId = parkingLotId;
        IncomingDateTime = inComingDateTime;
        OutgoingDateTime = outGoingDateTime;
        Status = reservationStatus;
        BusCampanyName = busCampanyName;
        CanceledAt = canceledAt;
    }

    public static ReservationEntity CreateNewReservation(
        Guid accountId,
        Guid parkingLotId,
        DateTime inComingDateTime,
        DateTime outGoingDateTime,
        ReservationStatus reservationStatus,
        string busCampanyName,
        DateTime? canceledAt)
    {
        return new ReservationEntity(
            Guid.CreateVersion7(),
            accountId,
            parkingLotId,
            inComingDateTime,
            outGoingDateTime,
            reservationStatus,
            busCampanyName,
            canceledAt
        );
    }
    
    public static ReservationEntity FromSnapshot(
        Guid id,
        Guid accountId,
        Guid parkingLotId,
        DateTime inComingDateTime,
        DateTime outGoingDateTime,
        ReservationStatus reservationStatus,
        string busCampanyName,
        DateTime? canceledAt)
    {
        return new ReservationEntity(
            id,
            accountId,
            parkingLotId,
            inComingDateTime,
            outGoingDateTime,
            reservationStatus,
            busCampanyName,
            canceledAt
        );
    }
}