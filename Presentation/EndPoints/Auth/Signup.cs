using BusReservationService.Domain.Entity;
using BusReservationService.Domain.Repository;
using BusReservationService.Models;
using BusReservationService.Services.Interface;
using BusReservationService.Util;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BusReservationService.Endpoints.Auth;


using ResponseType = Results<
    Created<SingupResponse>, 
    Conflict>;

internal class Signup(IRegistrationAccountService registrationAccountService) : Endpoint<SingupRequest, ResponseType>
{

    public override void Configure()
    {
        Verbs(Http.POST);
        Routes("/auth/singup");
        AllowAnonymous();
    }
    
    public override async Task<ResponseType> HandleAsync(SingupRequest req, CancellationToken c)
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
            new SingupResponse(account)
        );
    }
}

internal record SingupRequest(string Email, string Password);

internal class Validator : Validator<SingupRequest>
{
    public Validator()
    {

    }
}


internal record SingupResponse(AccountModel Account);