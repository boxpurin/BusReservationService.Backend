using BusReservationService.Domain.Entity;
using BusReservationService.Domain.Repository;
using BusReservationService.Models;
using BusReservationService.Util;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BusReservationService.Endpoints.Auth;


using ResponseType = Results<
    Created<SingupResponse>, 
    Conflict>;

internal class Singup(IAccountRepository AccountRepository) : Endpoint<SingupRequest, ResponseType>
{

    public override void Configure()
    {
        Verbs(Http.POST);
        Routes("/auth/singup");
        AllowAnonymous();
    }
    
    public override async Task<ResponseType> HandleAsync(SingupRequest req, CancellationToken c)
    {
        var account = await AccountRepository.FindByEmail(req.Email);
        if (account is not null)
        {
            return TypedResults.Conflict();
        }
        
        // TODO
        // リクエストのパスワードのハッシュ化

        var (hash , salt) = Argon2Helper.HashPassword(req.Password);
        AccountEntity entity = AccountEntity.CreateUserRoleAccount(req.Email, hash , salt);

        var result = await AccountRepository.AddAsync(entity);
        var sessionId = Guid.CreateVersion7().ToString();

        await CookieAuth.SignInAsync(u =>
        {
            u.Claims.Add(new("SessionId", sessionId));
        });

        return TypedResults.Created(
            $"/auth/signup/{result.Id}",
            new SingupResponse(new AccountModel(result))
        );
    }
}

internal record SingupRequest(string Email, string Password);

internal class Validator : Validator<SingupRequest>
{
    public Validator()
    {

    }
}


internal record SingupResponse(AccountModel Account);