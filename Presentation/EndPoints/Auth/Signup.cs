using BusReservationService.Models;
using BusReservationService.Services.Interface;
using FastEndpoints;
using FastEndpoints.Security;
using FluentValidation;
using Microsoft.AspNetCore.Http;


using ResponseType = Microsoft.AspNetCore.Http.HttpResults.Results<
    Microsoft.AspNetCore.Http.HttpResults.Created<BusReservationService.Endpoints.Auth.SignupResponse>,
    Microsoft.AspNetCore.Http.HttpResults.Conflict>;

namespace BusReservationService.Endpoints.Auth;
internal class Signup(IRegistrationAccountService registrationAccountService) : Endpoint<SignupRequest, ResponseType>
{

    public override void Configure()
    {
        Verbs(Http.POST);
        Routes("/auth/singup");
        AllowAnonymous();
    }
    
    public override async Task<ResponseType> ExecuteAsync(SignupRequest req, CancellationToken c)
    {
        var account = await registrationAccountService.RegistrationAccountAsync(req.Email, req.Password);
        if(account is null)
        {
            return TypedResults.Conflict();
        }

        await CookieAuth.SignInAsync(u =>
        {
            u["AccountId"] = account.Id.ToString()!;
        });

        return TypedResults.Created(
            $"/auth/signup/{account.Id}",
            new SignupResponse(account)
        );
    }
}

internal record SignupRequest(string Email, string Password);

internal class SignUpValidator : Validator<SignupRequest>
{
    public SignUpValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .NotNull()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .NotNull()
            .MinimumLength(8);
    }
}


internal record SignupResponse(AccountModel Account);