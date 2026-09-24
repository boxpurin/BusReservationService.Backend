using BusReservationService.Dto.Request;
using BusReservationService.Models;
using BusReservationService.Services.Interface;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BusReservationService.Endpoints.Auth;

using ResponseType = Results<Ok, BadRequest, InternalServerError>;
internal class Singin(IAccountService AccountService) : Endpoint<SinginRequest, ResponseType>
{
    public override void Configure()
    {
        Verbs(Http.POST);
        Routes("/auth/singin");
        AllowAnonymous();
    }

    public override async Task<ResponseType> HandleAsync(SinginRequest req, CancellationToken c)
    {
        var account = await AccountService.SinginAccount(req.Email, req.Password);

        if (account is null)
        {
            AddError("サインインに失敗しました。");
            return TypedResults.BadRequest();
        }

        var sessionId = Guid.CreateVersion7().ToString();

        await CookieAuth.SignInAsync(u =>
        {
            u.Roles.Add(account.AccountRole.ToString());
            u.Permissions.Add("RegistedUser");

            u.Claims.Add(new("SessionId", sessionId));
            u.Claims.Add(new("Status", account.AccountStatus.ToString()));
        });

        return TypedResults.Ok();
    }
}


public record SinginRequest(string Email, string Password);
