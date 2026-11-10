using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Http;

namespace BusReservationService.Endpoints.Auth;

/// <summary>
/// サインアウトを行うエンドポイントです。
/// Cookieから認証情報を削除します。
/// </summary>
internal class Signout() : EndpointWithoutRequest
{
    public override void Configure()
    {
        Verbs(Http.POST);
        Routes("/auth/signout");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken c)
    {
        await CookieAuth.SignOutAsync();

        await Send.NoContentAsync();
    }
}
