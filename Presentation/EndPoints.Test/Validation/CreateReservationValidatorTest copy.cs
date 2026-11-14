using BusReservationService.Endpoints.Test;
using BusReservationService.Endpoints.Reservation;
using FastEndpoints.Testing;
using Microsoft.Extensions.Options;
using Shouldly;
using FluentValidation.TestHelper;

namespace BusReservationService.Endpoints.Test.Validation;

public class CreateReservationRequestValidatorTestR
{
    private readonly CreateReservationRequestValidator _validator = new ();

    [Fact]
    public void Expected_Create_Reservation_Request()
    {
        var request = new CreateReservationRequest(
            Guid.NewGuid(),
            DateTime.Now,
            DateTime.Now.AddHours(1),
            "Bus Company"
        );
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void IncomingDateTime_Should_Be_Less_Than_OutgoingDateTime()
    {
        
        var dt = DateTime.Now;
        var lessThanRequest = new CreateReservationRequest(
            Guid.NewGuid(),
            dt.AddHours(1),
            dt,
            "Bus Company"
        );

        var result = _validator.TestValidate(lessThanRequest);
        result.ShouldHaveValidationErrorFor(x => x.IncomingDateTime);

        var equalRequest = new CreateReservationRequest(
            Guid.NewGuid(),
            dt,
            dt,
            "Bus Company"
        );

        result = _validator.TestValidate(equalRequest);
        result.ShouldHaveValidationErrorFor(x => x.IncomingDateTime);
    }
}