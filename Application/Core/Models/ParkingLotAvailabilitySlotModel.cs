namespace BusReservationService.Application.Core.Models;

public record ParkingLotAvailabilitySlotModel
{
    public DateTimeOffset StartTime { get; init; }
    public DateTimeOffset EndTime { get; init; }
    public int MaxReservationCapacity { get; init; }
    public int RemainingCapacity { get; init; }
}