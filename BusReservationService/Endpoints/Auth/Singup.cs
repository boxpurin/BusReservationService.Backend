using BusReservationService.Domain.DomainService;
using BusReservationService.Domain.ValueObject;
using BusReservationService.Dto.Request;
using BusReservationService.Models;
using BusReservationService.Services.Interface;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BusReservationService.Endpoints.Auth;


using ResponseType = Results<
    Created<SingupResponse>, 
    Conflict, 
    BadRequest>;

internal class Singup(IAccountService AccountRegistorationService) : Endpoint<SingupRequest, ResponseType>
{

    public override void Configure()
    {
        Verbs(Http.POST);
        Routes("/auth/singup");
        AllowAnonymous();
    }
    
    public override async Task<ResponseType> HandleAsync(SingupRequest req, CancellationToken c)
    {
        var account = await AccountRegistorationService.RegistorationAccount(
            new RegistrationAccount(req.Email, req.Password, AccountRole.User)
            );

        if (account is null)
        {
            return TypedResults.BadRequest();
        }

        var sessionId = Guid.CreateVersion7().ToString();

        await CookieAuth.SignInAsync(u =>
        {
            u.Roles.Add("User");
            u.Permissions.Add("RegistedUser");

            u.Claims.Add(new("SessionId", sessionId));
        });

        return TypedResults.Created(
            $"/auth/signup/{account.Id}",
            new SingupResponse(new AccountModel(account))
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