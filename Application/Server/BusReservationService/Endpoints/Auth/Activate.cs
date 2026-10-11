namespace BusReservationService.Endpoints.Auth;

using BusReservationService.Domain.Entity;
using BusReservationService.Domain.Repository;
using BusReservationService.Domain.ValueObject;
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
public class Activate(
    ISessionService<Guid, SessionValue> SessionService, 
    IAccountRepository AccountRepository,
    IUserRepository UserRepository) : Endpoint<ActivateRequest>
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
        // セッション情報の習得
        var session = SessionService[req.SessionId];
        if (session is null)
        {
            await Send.ErrorsAsync(401);
            return;
        }

        // アカウント情報を習得
        var account = await AccountRepository.FindByAccountId(session.AccountId);
        if (account is null)
        {
            AddError("アカウント情報が存在しません。");
            await Send.ErrorsAsync(401);
            return;
        }

        // すでにユーザー情報が追加されているアカウントIDなのかを確認する
        if (await UserRepository.ExistByAccountId(session.AccountId))
        {
            AddError("このアカウントにはユーザー情報がすでに登録されています。");
            await Send.ErrorsAsync(415);
            return;
        }

        var entity = UserEntity.CreateUserEntity(
            session.AccountId, 
            req.FirstName, 
            req.LastName,
            req.ZipCode,
            req.StreetAdress,
            req.PhoneNumber,
            req.FaxNumber,
            req.UserType
        );

        // ユーザーの登録
        var user = await UserRepository.AddAsymc(new AddAsymcDto(
            session.AccountId, 
            req.FirstName, 
            req.LastName,
            req.ZipCode,
            req.StreetAdress,
            req.PhoneNumber,
            req.FaxNumber,
            req.UserType
        ));

        // アカウントの有効化
        account.Activate();
        // アカウント情報の更新
        account = await AccountRepository.UpdateAsync(account);

        // セッションの更新
        var sessionValue = new SessionValue(account, user);
        SessionService[req.SessionId] = sessionValue;

        await Send.CreatedAtAsync("/auth/activate");
    }
}

public record ActivateRequest(
    string FirstName, 
    string LastName, 
    string ZipCode,
    string StreetAdress,
    string PhoneNumber, 
    string? FaxNumber,
    UserType UserType)
{
    [FromClaim]
    public Guid SessionId {get; set;}
}