using BusReservationService.Domain.Repository;
using BusReservationService.Models;
using BusReservationService.Services.Interface;
using BusReservationService.Util;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BusReservationService.Endpoints.Auth;

using ResponseType = Results<Ok, UnauthorizedHttpResult, InternalServerError>;
internal class Singin(
    IAccountUserQueryService accountUserQueryService, 
    ISessionService<Guid, SessionValue> SessionService) : Endpoint<SinginRequest, ResponseType>
{
    public override void Configure()
    {
        Verbs(Http.POST);
        Routes("/auth/singin");
        AllowAnonymous();
    }

    public override async Task<ResponseType> HandleAsync(SinginRequest req, CancellationToken c)
    {
        
        var accountUser = await accountUserQueryService.GetModelByEmailAsync(req.Email);
        if (accountUser is null)
        {
            AddError("サインインに失敗しました。");
            return TypedResults.Unauthorized();
        }

        if(!Argon2Helper.VerifyPassword(req.Password, accountUser.PasswordHash, accountUser.PasswordSalt))
        {
            AddError("サインインに失敗しました。");
            return TypedResults.Unauthorized();            
        }

        var sessionValue = new SessionValue(accountUser);
        var sessionId = SessionService.CreateRandomKeySession(sessionValue);

        await CookieAuth.SignInAsync(u =>
        {
            u["SessionId"] = sessionId.ToString();
        });

        return TypedResults.Ok();
    }
}


public record SinginRequest(string Email, string Password);
