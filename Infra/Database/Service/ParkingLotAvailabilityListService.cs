using BusReservationService.Application.Core.Models;
using BusReservationService.Application.Core.Services.Interface;
using BusReservationService.Domain.Repository;
using BusReservationService.Share;
using BusReservationService.Share.Utils.Error;

namespace BusReservationService.Infra.Database.Service;

public class ParkingLotAvailabilityListService(
    IReservationRepository reservationRepository,
    IParkingLotRepository parkingLotRepository,
    ParkingLotAvailabilityOptions options
    ) : IParkingLotAvailabilityListService
{
    public async Task<ApplicationResult<ParkingLotAvailability, Error>> GetDailyAvailabilityAsync(Guid parkingLotId, DateOnly date)
    {
        try
        {
            var parkingLot = await parkingLotRepository.FindByParkingLotIdAsync(parkingLotId);
            if (parkingLot == null)
            {
                return new Err<ParkingLotAvailability, Error>(
                    new Error {
                        Message = $"Parking lot with ID {parkingLotId} not found.",
                        ErrorCode = "PARKING_LOT_NOT_FOUND",
                        StatusCode = 404
                    });
            }

            var availability = new ParkingLotAvailability
            {
                ParkingLotId = parkingLotId,
                Date = date,
                SlotIntervalMinutes = options.SlotInterval
            };

            var from_dt = date.ToDateTime(TimeOnly.MinValue);
            var to_dt = date.ToDateTime(TimeOnly.MaxValue);

            var overlappingReservations = await reservationRepository.FindOverlappingByParkingLotIdAsync(
                parkingLotId,
                from_dt,
                to_dt
            );

            var slots = new List<ParkingLotAvailabilitySlotModel>();
            
            for (var time = from_dt; time < to_dt; time = time.AddMinutes(availability.SlotIntervalMinutes.TotalMinutes))
            {
                var slotStartTime = time;
                var slotEndTime = time.AddMinutes(availability.SlotIntervalMinutes.TotalMinutes);

                var overlappingSlotReservations = overlappingReservations
                    .Where(r => r.IncomingDateTime < slotEndTime && r.OutgoingDateTime > slotStartTime)
                    .ToList();

                slots.Add(new ParkingLotAvailabilitySlotModel
                {
                    StartTime = slotStartTime,
                    EndTime = slotEndTime,
                    MaxReservationCapacity = parkingLot.ReservationCapacity,
                    RemainingCapacity = parkingLot.ReservationCapacity - overlappingSlotReservations.Count
                });
            }
            
            availability = availability with { AvailabilitySlots = slots };

            return new Ok<ParkingLotAvailability, Error>(availability);
        }
        catch
        {
            // Handle exception or log it
            throw;
        }
    }
}