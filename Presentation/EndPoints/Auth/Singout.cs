using Microsoft.AspNetCore.Http.HttpResults;
using FastEndpoints;
using FastEndpoints.Security;
using BusReservationService.Services.Interface;
using BusReservationService.Models;
using Microsoft.AspNetCore.Http;

namespace BusReservationService.Endpoints.Auth;

using ResponseType = Results<NoContent, BadRequest, InternalServerError>;
public class Singout() : Endpoint<SingoutRequest, ResponseType>
{    public override void Configure()
    {
        Verbs(Http.POST);
        Routes("/auth/singout");
        AllowAnonymous();
    }

    public override async Task<ResponseType> HandleAsync(SingoutRequest req,CancellationToken c)
    {
        await CookieAuth.SignOutAsync();
        return TypedResults.NoContent();
    }
}

public record SingoutRequest()
{
    [FromClaim]
    public Guid AccountId {get; set;}
}

