using BusReservationService.Domain.Entity;
using BusReservationService.Domain.ValueObject.ReservationStatus;
namespace BusReservationService.Models;

public class ReservationModel
{
    public Guid Id { get; set; }
    public string ParkingLotName { get; set; } = string.Empty;
    public ReservationStatus ReservationStatus { get; set; }
    public DateTime IncomingDateTime { get; set; }
    public DateTime OutgoingDateTime { get; set; }
    public string BusCampanyName { get; set; } = string.Empty;

    public ReservationModel(){}

    public ReservationModel(ReservationEntity reservation, ParkingLotEntity parkingLot)
    {
        Id = reservation.Id;
        ParkingLotName = parkingLot.ParkingLotName;
        IncomingDateTime = reservation.IncomingDateTime;
        OutgoingDateTime = reservation.OutgoingDateTime;
        ReservationStatus = reservation.ReservationStatus;
        BusCampanyName = reservation.BusCampanyName;
    }
}