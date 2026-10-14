using System.ComponentModel.DataAnnotations.Schema;
using BusReservationService.Domain.Model;

namespace BusReservationService.Domain.Entity;

public class ParkingLotEntity
{
    public Guid Id { get; init; }
    public string ParkingLotName { get; private set; }
    public Prefecture Prefecture { get; private set; }
    public string ZipCode { get; private set; } 
    public string StreetAddress { get; private set; }
    public string PhoneNumber { get; private set; }
    public string? FaxNumber { get; private set; }
    public short MaxCapacity { get; private set; }
    public short ReservationCapacity { get; private set; }
    public bool IsAllowsBoardingAlighting { get; private set; }
    public bool IsActive { get; private set;}

    private ParkingLotEntity(
        Guid id,
        string parkingLotName,
        string zipCode,
        Prefecture prefecture,
        string streetAddress,
        string phoneNumber,
        string? faxNumber,
        short maxCapacity,
        short reservationCapacity,
        bool isAllowsBoardingAlighting,
        bool isActive)
    {
        Id = id;
        ParkingLotName = parkingLotName;
        ZipCode = zipCode;
        Prefecture = prefecture;
        StreetAddress = streetAddress;
        PhoneNumber = phoneNumber;
        FaxNumber = faxNumber;
        MaxCapacity = maxCapacity;
        ReservationCapacity = reservationCapacity;
        IsAllowsBoardingAlighting = isAllowsBoardingAlighting;
        IsActive = isActive;
    }

    public static ParkingLotEntity CreateNewParkingLotEntity(
        string parkingLotName,
        string zipCode,
        Prefecture prefecture,
        string streetAddress,
        string phoneNumber,
        string? faxNumber,
        short maxCapacity,
        short reservationCapacity,
        bool isAllowsBoardingAlighting = false,
        bool isActive = false)
    {
        return new ParkingLotEntity(
            Guid.NewGuid(),
            parkingLotName,
            zipCode,
            prefecture,
            streetAddress,
            phoneNumber,
            faxNumber,
            maxCapacity,
            reservationCapacity,
            isAllowsBoardingAlighting,
            isActive
        );
    }
}