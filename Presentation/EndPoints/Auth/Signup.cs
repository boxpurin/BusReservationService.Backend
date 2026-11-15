using BusReservationService.Models;
using BusReservationService.Services.Interface;
using BusReservationService.Share;
using BusReservationService.Share.Utils.Error;
using BusReservationService.Stores.Interface;
using FastEndpoints;
using FastEndpoints.Security;
using FluentValidation;
namespace BusReservationService.Endpoints.Auth;
internal class Signup(
    IRegistrationAccountService registrationAccountService, 
    IActivationSessionStore activationSessionStore) : Endpoint<SignupRequest, SignupResponse>
{

    public override void Configure()
    {
        Verbs(Http.POST);
        Routes("/auth/signup");
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(SignupRequest req, CancellationToken c)
    {
        var result = await registrationAccountService.RegistrationAccountAsync(req.Email, req.Password);
        if(result is Err<AccountModel, Error> err)
        {
            AddError(err.Value.Message, err.Value.ErrorCode);
            await Send.ErrorsAsync(err.Value.StatusCode);
            return;
        }

        var account = result.Unwrap();
        var activationSessionId = await activationSessionStore.CreateAsync(account.Id, c);
        
        await CookieAuth.SignInAsync(u =>
        {
            u["ActivationSessionId"] = activationSessionId;
            u.Roles.Add("User");
        });

        await Send.CreatedAtAsync<User.GetUserProfile>(
            responseBody: new SignupResponse(account),
            cancellation: c
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