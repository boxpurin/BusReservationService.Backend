using ReservationStatusDomain = BusReservationService.Domain.ValueObject.ReservationStatus;

namespace BusReservationService.Database.Enums;

public enum ReservationStatus
{
    Pending,
    Confirmed,
    Completed,
    Canceled,
}

public static class ReservationStatusExtension
{
    public static ReservationStatusDomain ToDomain(this ReservationStatus status) 
    => Enum.Parse<ReservationStatusDomain>(status.ToString());
}
