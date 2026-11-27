namespace BusReservationService.Application.Core.Models;

public record ParkingLotAvailability
{
    public Guid ParkingLotId { get; init; }
    public DateOnly Date { get; init; }
    public TimeSpan SlotIntervalMinutes { get; init; } = TimeSpan.FromMinutes(15);
    public IReadOnlyList<ParkingLotAvailabilitySlotModel> AvailabilitySlots { get; init; } = [];
}