using BusReservationService.Domain.ValueObject.UserType;
using BusReservationService.Models;
using BusReservationService.Services.Interface;
using BusReservationService.Share;
using BusReservationService.Share.Utils.Error;
using BusReservationService.Stores.Interface;
using FastEndpoints;
using FastEndpoints.Security;
using FluentValidation;

namespace BusReservationService.Endpoints.Auth;



/// <summary>
/// 作成済みアカウントに対して初回のユーザー情報を送信し、アカウントを有効化する.
/// - METHOD POST
/// - Route /auth/activate
/// </summary>
/// <param name="SessionService">セッション管理</param>
/// <param name="AccountRepository">アカウント情報のリポジトリ</param>
/// <param name="UserRepository">ユーザー情報のリポジトリ</param>
internal class Activate(
    IOnboardUserProfileService onboardUserProfileService,
    IActivationSessionStore activationSessionStore) : Endpoint<ActivateRequest, ActivateResponse>
{
    public override void Configure()
    {
        Verbs(Http.POST);
        Routes("/auth/activate");
        Claims("ActivationSessionId");
    }

    public override async Task HandleAsync(ActivateRequest req, CancellationToken c)
    {
        var accountId = await activationSessionStore.FindAccountIdAsync(req.ActivationSessionId, c);

        if(accountId == Guid.Empty)
        {
            AddError("Activation session not found", "InvalidSession");
            await Send.ErrorsAsync(401);
            return;
        }

        var result = await onboardUserProfileService.OnboardUserProfile(
            accountId,
            req.FirstName,
            req.LastName,
            req.ZipCode,
            req.StreetAddress,
            req.PhoneNumber,
            req.FaxNumber,
            req.UserType
        );

        if (result is Err<AccountModel, Error> err)
        {
            AddError(err.Value.Message, err.Value.ErrorCode);
            await Send.ErrorsAsync(err.Value.StatusCode);
            return;
        }

        await activationSessionStore.DeleteAsync(req.ActivationSessionId, c);

        await Send.CreatedAtAsync(
            $"/user/profile",
            responseBody: new ActivateResponse(result.Unwrap()),
            cancellation: c
        );
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
    public string ActivationSessionId {get; set;} = string.Empty;
}

internal class ActivateValidator : Validator<ActivateRequest>
{
    public ActivateValidator()
    {
        RuleFor(x => x.ActivationSessionId)
            .NotEmpty();

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.ZipCode)
            .Length(7);
        
        RuleFor(x => x.StreetAddress)
            .NotEmpty();

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .MaximumLength(15);

        RuleFor(x => x.FaxNumber)
            .MaximumLength(15);
        
        RuleFor(x => x.UserType)
            .IsInEnum();
    }
}

internal record ActivateResponse(AccountModel Account);