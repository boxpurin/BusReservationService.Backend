using BusReservationService.Dto.Request;
using BusReservationService.Models;
using BusReservationService.Services.Interface;
using FastEndpoints;
using FastEndpoints.Security;

namespace BusReservationService.Endpoints.Sandbox;

internal class CookieTest(ISessionService SessionService) : Endpoint<CookieTestRequest, CookieTestResponse>
{

    public override void Configure()
    {
        Verbs(Http.POST);
        Routes("/api/cookie");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CookieTestRequest req, CancellationToken c)
    {
        await CookieAuth.SignInAsync(u =>
        {
            Guid accountId = Guid.CreateVersion7();
            var key = SessionService.CreateSession(accountId);

            u.Roles.Add("User");
            u["SessionId"] = key.Value.ToString()!;
        });

        await Send.OkAsync(new CookieTestResponse (Email : req.Email, Password : req.Password));
    }
}

internal record CookieTestRequest(string Email, string Password);

internal class Validator : Validator<CookieTestRequest>
{
    public Validator()
    {

    }
}

internal record CookieTestResponse(string Email, string Password);