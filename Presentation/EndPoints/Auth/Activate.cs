namespace BusReservationService.Endpoints.Auth;

using BusReservationService.Domain.Entity;
using BusReservationService.Domain.Repository;
using BusReservationService.Domain.ValueObject.UserType;
using BusReservationService.Models;
using BusReservationService.Services.Interface;
using FastEndpoints;

/// <summary>
/// 作成済みアカウントに対して初回のユーザー情報を送信し、アカウントを有効化する.
/// - METHOD POST
/// - Route /auth/activate
/// </summary>
/// <param name="SessionService">セッション管理</param>
/// <param name="AccountRepository">アカウント情報のリポジトリ</param>
/// <param name="UserRepository">ユーザー情報のリポジトリ</param>
public class Activate(IOnboardUserProfileService onboardUserProfileService) : Endpoint<ActivateRequest, ActivateResponse>
{
    public override void Configure()
    {
        Verbs(Http.POST);
        Routes("/auth/activate");
        Claims("SessionId");
        AllowAnonymous();
    }

    public override async Task HandleAsync(ActivateRequest req, CancellationToken c)
    {
        await onboardUserProfileService.OnboardUserProfile(
            req.AccountId,
            req.FirstName,
            req.LastName,
            req.ZipCode,
            req.StreetAddress,
            req.PhoneNumber,
            req.FaxNumber,
            req.UserType
        );

        await Send.CreatedAtAsync("/auth/activate", new ActivateResponse());
    }
}

public record ActivateRequest(
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

public record ActivateResponse();