using BusReservationService.Domain.Entity;
using BusReservationService.Domain.ValueObject.Prefecture;
namespace BusReservationService.Models;

public class ParkingLotProfile
{
    public Guid Id { get; init; }
    public string ParkingLotName { get; private set; } = string.Empty;
    public string Prefecture { get; private set; } = string.Empty;
    public string ZipCode { get; private set; } = string.Empty;
    public string StreetAddress { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public string? FaxNumber { get; private set; }
    public short MaxCapacity { get; private set; }
    public short ReservationCapacity { get; private set; }
    public bool IsAllowsBoardingAlighting { get; private set; }
    public bool IsActive { get; private set;}

    public ParkingLotProfile()
    {

    }

    public static ParkingLotProfile FromEntity(ParkingLotEntity entity)
    {
        return new ParkingLotProfile
        {
            Id = entity.Id,
            ParkingLotName = entity.ParkingLotName,
            Prefecture = entity.Prefecture.ToNameJis(),
            ZipCode = entity.ZipCode,
            StreetAddress = entity.StreetAddress,
            PhoneNumber = entity.PhoneNumber,
            FaxNumber = entity.FaxNumber,
            MaxCapacity = entity.MaxCapacity,
            ReservationCapacity = entity.ReservationCapacity,
            IsAllowsBoardingAlighting = entity.IsAllowsBoardingAlighting,
            IsActive = entity.IsActive
        };
    }
}

