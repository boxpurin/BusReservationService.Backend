using ReservationStatusDomain = BusReservationService.Domain.ValueObject.ReservationStatus;

namespace BusReservationService.Database.Enums;

internal enum ReservationStatus
{
    Pending,
    Confirmed,
    Completed,
    Canceled,
}

internal static class ReservationStatusExtension
{
    public static ReservationStatusDomain ToDomain(this ReservationStatus status) 
    => Enum.Parse<ReservationStatusDomain>(status.ToString());
}
