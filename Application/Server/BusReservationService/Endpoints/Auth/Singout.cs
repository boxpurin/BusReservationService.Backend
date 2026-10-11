using Microsoft.AspNetCore.Http.HttpResults;
using FastEndpoints;
using FastEndpoints.Security;
using BusReservationService.Services.Interface;
using BusReservationService.Models;

namespace BusReservationService.Endpoints.Auth;

using ResponseType = Results<NoContent, BadRequest, InternalServerError>;
public class Singout(ISessionService<Guid, SessionValue> SessionService) : Endpoint<SingoutRequest, ResponseType>
{    public override void Configure()
    {
        Verbs(Http.POST);
        Routes("/auth/singout");
        AllowAnonymous();
    }

    public override async Task<ResponseType> HandleAsync(SingoutRequest req,CancellationToken c)
    {
        await CookieAuth.SignOutAsync();

        SessionService.ClearSession(req.SessionId);
        return TypedResults.NoContent();
    }
}

public record SingoutRequest()
{
    [FromClaim]
    public Guid SessionId {get; set;}
}

