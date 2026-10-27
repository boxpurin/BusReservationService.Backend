using BusReservationService.Domain.ValueObject.UserType;
using BusReservationService.Models;
using BusReservationService.Services.Interface;
using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using ResponseType = Microsoft.AspNetCore.Http.HttpResults.Results<
    Microsoft.AspNetCore.Http.HttpResults.Created<BusReservationService.Endpoints.Auth.ActivateResponse>,
    Microsoft.AspNetCore.Http.HttpResults.UnauthorizedHttpResult,
    Microsoft.AspNetCore.Http.HttpResults.InternalServerError
>;
namespace BusReservationService.Endpoints.Auth;



/// <summary>
/// 作成済みアカウントに対して初回のユーザー情報を送信し、アカウントを有効化する.
/// - METHOD POST
/// - Route /auth/activate
/// </summary>
/// <param name="SessionService">セッション管理</param>
/// <param name="AccountRepository">アカウント情報のリポジトリ</param>
/// <param name="UserRepository">ユーザー情報のリポジトリ</param>
internal class Activate(IOnboardUserProfileService onboardUserProfileService) : Endpoint<ActivateRequest, ResponseType>
{
    public override void Configure()
    {
        Verbs(Http.POST);
        Routes("/auth/activate");
        Claims("AccountId");
        AllowAnonymous();
    }

    public override async Task<ResponseType> ExecuteAsync(ActivateRequest req, CancellationToken c)
    {
        var account = await onboardUserProfileService.OnboardUserProfile(
            req.AccountId,
            req.FirstName,
            req.LastName,
            req.ZipCode,
            req.StreetAddress,
            req.PhoneNumber,
            req.FaxNumber,
            req.UserType
        );

        if (account is null)
        {
            await Send.ErrorsAsync(500);
            AddError("ユーザー情報の登録に失敗しました");
            return TypedResults.InternalServerError();
        }

        return TypedResults.Created("/auth/activate", new ActivateResponse(account));
    }
}

internal record ActivateRequest(
    string FirstName,
    string LastName,
    string ZipCode,
    string StreetAddress,
    string PhoneNumber,
    string? FaxNumber,
    UserType UserType)
{
    [FromClaim]
    public Guid AccountId {get; set;}
}

internal class ActivateValidator : Validator<ActivateRequest>
{
    public ActivateValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty()
            .Must(x => x.Version == 7);

        RuleFor(x => x.FirstName)
            .NotEmpty();

        RuleFor(x => x.LastName)
            .NotEmpty();

        RuleFor(x => x.ZipCode)
            .Length(7);
        
        RuleFor(x => x.StreetAddress)
            .NotEmpty();

        RuleFor(x => x.PhoneNumber)
            .NotEmpty();

        RuleFor(x => x.FaxNumber);
        
        RuleFor(x => x.UserType)
            .IsInEnum();
    }
}

internal record ActivateResponse(AccountModel Account);