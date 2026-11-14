using BusReservationService.Endpoints.Test;
using BusReservationService.Endpoints.Reservation;
using FastEndpoints.Testing;
using Microsoft.Extensions.Options;
using Shouldly;
using FluentValidation.TestHelper;

namespace BusReservationService.Endpoints.Test.Validation;

public class UpdateReservationRequestValidatorTestR
{
    private readonly UpdateReservationRequestValidator _validator = new ();

    [Fact]
    public void Expected_Update_Reservation_Request()
    {
        var request = new UpdateReservationRequest(
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
        var lessThanRequest = new UpdateReservationRequest(
            Guid.NewGuid(),
            dt.AddHours(1),
            dt,
            "Bus Company"
        );

        var result = _validator.TestValidate(lessThanRequest);
        result.ShouldHaveValidationErrorFor(x => x.IncomingDateTime);

        var equalRequest = new UpdateReservationRequest(
            Guid.NewGuid(),
            dt,
            dt,
            "Bus Company"
        );

        result = _validator.TestValidate(equalRequest);
        result.ShouldHaveValidationErrorFor(x => x.IncomingDateTime);
    }
}