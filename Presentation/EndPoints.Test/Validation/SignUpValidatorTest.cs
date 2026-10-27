using BusReservationService.Endpoints.Test;
using BusReservationService.Endpoints.Auth;
using FastEndpoints.Testing;
using Microsoft.Extensions.Options;
using Shouldly;
using FluentValidation.TestHelper;

namespace BusReservationService.Endpoints.Test.Validation;

public class SignUpValidatorTest
{
    private readonly SignUpValidator _validator = new();

    [Fact]
    public void Should_Not_Have_Error_When_Valid_Email_And_Sufficient_Password()
    {
        var model = new SignupRequest(
            "valid@email.com",
            "suFF1c1entLonGp@ssw0rd"
        );

        var validate = _validator.TestValidate(model);
        validate.ShouldNotHaveAnyValidationErrors();

    }
    
    [Fact]
    public void Should_Have_Error_When_Invalid_Email_Format()
    {
        var model = new SignupRequest(
            "invalidemail",
            "suFF1c1entLonGp@ssw0rd"
        );

        var validate = _validator.TestValidate(model);
        validate.ShouldHaveValidationErrors();
    }
    
    [Fact]
    public void Should_Have_Error_When_Short_Password()
    {
        var model = new SignupRequest(
            "valid@email.com",
            "short"
        );

        var validate = _validator.TestValidate(model);
        validate.ShouldHaveValidationErrors();
    }
}