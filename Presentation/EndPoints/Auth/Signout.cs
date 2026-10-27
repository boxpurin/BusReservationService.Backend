using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Http;

using ResponseType = Microsoft.AspNetCore.Http.HttpResults.Results<
    Microsoft.AspNetCore.Http.HttpResults.NoContent,
    Microsoft.AspNetCore.Http.HttpResults.BadRequest,
    Microsoft.AspNetCore.Http.HttpResults.InternalServerError>;

namespace BusReservationService.Endpoints.Auth;
internal class Signout() : Endpoint<SignOutRequest, ResponseType>
{
    public override void Configure()
    {
        Verbs(Http.POST);
        Routes("/auth/singout");
        AllowAnonymous();
    }

    public override async Task<ResponseType> ExecuteAsync(SignOutRequest req,CancellationToken c)
    {
        await CookieAuth.SignOutAsync();
        return TypedResults.NoContent();
    }
}

internal record SignOutRequest()
{
    [FromClaim]
    public Guid AccountId {get; set;}
}

internal record SignOutResponse();

