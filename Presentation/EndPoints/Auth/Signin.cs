using BusReservationService.Domain.ValueObject.AccountRole;
using BusReservationService.Domain.ValueObject.AccountStatus;
using BusReservationService.Domain.ValueObject.UserType;
using BusReservationService.Models;
using BusReservationService.Services.Interface;
using BusReservationService.Share;
using BusReservationService.Share.Utils.Error;
using BusReservationService.Util;
using FastEndpoints;
using FastEndpoints.Security;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace BusReservationService.Endpoints.Auth;

/// <summary>
/// サインインエンドポイント
/// </summary>
/// <param name="authAccountService">認証アカウントサービスのインスタンス</param>
internal class Signin(
    IAuthAccountService authAccountService) : Endpoint<SignInRequest, SignInResponse>
{
    public override void Configure()
    {
        Verbs(Http.POST);
        Routes("/auth/signin");
        AllowAnonymous();
    }

    public override async Task HandleAsync(SignInRequest req, CancellationToken c)
    {
        var accountUserResult = await authAccountService.SignInAccountAsync(req.Email, req.Password);
        if (accountUserResult is Err<AccountUserModel, Error> err)
        {
            AddError(err.Value.Message, err.Value.ErrorCode);
            await Send.ErrorsAsync(err.Value.StatusCode);
            return;
        }

        var accountUser = accountUserResult.Unwrap();
        await CookieAuth.SignInAsync(u =>
        {
            u["AccountId"] = accountUser.AccountId.ToString();
            u.Roles.Add(accountUser.AccountRole.ToString());
        });

        await Send.OkAsync(new SignInResponse(
            accountUser.Email,
            accountUser.FirstName,
            accountUser.LastName,
            accountUser.AccountRole,
            accountUser.AccountStatus,
            accountUser.UserType
        ));
    }
}


internal record SignInRequest(string Email, string Password);

internal class SignInRequestValidator : Validator<SignInRequest>
{
    public SignInRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}
internal record SignInResponse(
    string Email, string FirstName, string LastName, AccountRole Role, AccountStatus Status, UserType UserType
);
