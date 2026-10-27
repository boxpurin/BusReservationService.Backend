using BusReservationService.Services.Interface;
using BusReservationService.Util;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Http;

using ResponseType = Microsoft.AspNetCore.Http.HttpResults.Results<
    Microsoft.AspNetCore.Http.HttpResults.Ok,
    Microsoft.AspNetCore.Http.HttpResults.UnauthorizedHttpResult,
    Microsoft.AspNetCore.Http.HttpResults.InternalServerError>;

namespace BusReservationService.Endpoints.Auth;
internal class Signin(
    IAccountUserQueryService accountUserQueryService) : Endpoint<SignInRequest, ResponseType>
{
    public override void Configure()
    {
        Verbs(Http.POST);
        Routes("/auth/singin");
        AllowAnonymous();
    }

    public override async Task<ResponseType> ExecuteAsync(SignInRequest req, CancellationToken c)
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

        await CookieAuth.SignInAsync(u =>
        {
            u["AccountId"] = accountUser.AccountId.ToString();
        });

        return TypedResults.Ok();
    }
}


internal record SignInRequest(string Email, string Password);
internal record SignInResponse();
