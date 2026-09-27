namespace BusReservationService.Domain.ValueObject;

public enum ReservationStatus
{
    Pending = 0,
    Confirmed = 1,
    Completed = 2,
    Canceled = 3,
}

public static class ReservationStatusExtension
{
    public static bool IsPending(this ReservationStatus status) => status is ReservationStatus.Pending; 
    public static bool IsConfirmed(this ReservationStatus status) => status is ReservationStatus.Confirmed; 
    public static bool IsCompleted(this ReservationStatus status) => status is ReservationStatus.Completed; 
    public static bool IsCanceled(this ReservationStatus status) => status is ReservationStatus.Canceled; 
}