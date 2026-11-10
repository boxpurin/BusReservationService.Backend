using BusReservationService.Domain.ValueObject.ReservationStatus;

namespace BusReservationService.Domain.Entity;

public class ReservationEntity
{
    public Guid Id { get; private set; }
    public Guid AccountId { get; private set; }
    public Guid ParkingLotId { get; private set; }
    public DateTime IncomingDateTime { get; private set; }
    public DateTime OutgoingDateTime { get; private set; }
    public ReservationStatus ReservationStatus { get; private set; }
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
        ReservationStatus = reservationStatus;
        BusCampanyName = busCampanyName;
        CanceledAt = canceledAt;
    }

    public static ReservationEntity CreateNewReservation(
        Guid accountId,
        Guid parkingLotId,
        DateTime inComingDateTime,
        DateTime outGoingDateTime,
        string busCampanyName)
    {
        return new ReservationEntity(
            Guid.CreateVersion7(),
            accountId,
            parkingLotId,
            inComingDateTime,
            outGoingDateTime,
            ReservationStatus.Confirmed,
            busCampanyName,
            null
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

    public void Confirm()
    {
        ReservationStatus = ReservationStatus.Confirmed;
    }

    public void Cancel()
    {
        ReservationStatus = ReservationStatus.Canceled;
        CanceledAt = DateTime.Now;
    }
}