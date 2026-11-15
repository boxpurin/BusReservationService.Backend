using BusReservationService.Endpoints.Test;
using BusReservationService.Endpoints.Auth;
using FastEndpoints.Testing;
using Microsoft.Extensions.Options;
using Shouldly;
using FluentValidation.TestHelper;
using BusReservationService.Endpoints.Administrator.ParkingLots;
using BusReservationService.Domain.ValueObject.Prefecture;

namespace BusReservationService.Endpoints.Test.Validation;

public class UpdateParkingLotValidatorTest
{
    UpdateParkingLotValidator _validator = new UpdateParkingLotValidator();

    [Fact]
    public void Expected_UpdateParkingLotValidatorTest()
    {
        UpdateParkingLotRequest request = new UpdateParkingLotRequest(
            "Test Parking Lot",
            Prefecture.Tokyo,
            "1234567",
            "Test Street Address",
            "1234567890",
            "1234567891",
            10,
            5,
            true,
            true
        );
        request.AccountId = Guid.CreateVersion7();
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ReservationCapacity_Is_Should_Be_Less_Than_MaxCapacity()
    {
        UpdateParkingLotRequest request = new UpdateParkingLotRequest(
            "Test Parking Lot",
            Prefecture.Tokyo,
            "1234567",
            "Test Street Address",
            "1234567890",
            "1234567891",
            10,
            15,
            true,
            true
        );
        request.AccountId = Guid.CreateVersion7();
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.ReservationCapacity);
    }

    
    [Theory]
    [InlineData(0)]
    [InlineData(48)]
    public void Prefecture_Should_Be_Range_In_1_to_47(byte prefecture)
    {
        UpdateParkingLotRequest request = new UpdateParkingLotRequest(
            "Test Parking Lot",
            unchecked((Prefecture)prefecture),
            "1234567",
            "Test Street Address",
            "1234567890",
            "1234567891",
            10,
            15,
            true,
            true
        );
        request.AccountId = Guid.CreateVersion7();
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Prefecture);
    }

}