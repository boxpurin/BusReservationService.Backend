using BusReservationService.Endpoints.Test;
using BusReservationService.Endpoints.Auth;
using FastEndpoints.Testing;
using Microsoft.Extensions.Options;
using Shouldly;
using FluentValidation.TestHelper;

namespace BusReservationService.Endpoints.Test.Validation;

public class ActivateValidatorTest
{
    private readonly ActivateValidator _validator = new();

    [Fact]
    public void Test()
    {
        var model = new ActivateRequest(
            "FisrtName",
            "LastName",
            "1234567",
            "東京都千代田区1-1-1",
            "0668785723",
            null,
            Domain.ValueObject.UserType.UserType.Personal
        )
        {
            AccountId = Guid.CreateVersion7()
        };

        var validate = _validator.TestValidate(model);
        validate.ShouldNotHaveAnyValidationErrors();

    }
}