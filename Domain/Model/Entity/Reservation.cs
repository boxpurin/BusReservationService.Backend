using BusReservationService.Domain.ValueObject.ReservationStatus;
using BusReservationService.Domain.ValueObject.Unit;

namespace BusReservationService.Domain.Entity;

public class ReservationEntity
{
    public Guid Id { get; private set; }
    public ReservationNumberUnit ReservationNumber { get; private set; }
    public Guid AccountId { get; private set; }
    public Guid ParkingLotId { get; private set; }
    public DateTime IncomingDateTime { get; private set; }
    public DateTime OutgoingDateTime { get; private set; }
    public ReservationStatus ReservationStatus { get; private set; }
    public string BusCampanyName { get; private set; }
    public DateTime? CanceledAt { get; private set; }

    private ReservationEntity(
        Guid id,
        ReservationNumberUnit reservationNumber,
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
        ReservationNumber = reservationNumber;
        AccountId = accountId;
        ParkingLotId = parkingLotId;
        IncomingDateTime = inComingDateTime;
        OutgoingDateTime = outGoingDateTime;
        ReservationStatus = reservationStatus;
        BusCampanyName = busCampanyName;
        CanceledAt = canceledAt;
    }

    public static ReservationEntity CreateNewReservation(
        ReservationNumberUnit reservationNumber,
        Guid accountId,
        Guid parkingLotId,
        DateTime inComingDateTime,
        DateTime outGoingDateTime,
        string busCampanyName)
    {
        return new ReservationEntity(
            Guid.CreateVersion7(),
            reservationNumber,
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
        ReservationNumberUnit reservationNumber,
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
            reservationNumber,
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
        CanceledAt = DateTime.UtcNow;
    }
}