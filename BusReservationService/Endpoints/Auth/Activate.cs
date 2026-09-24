namespace BusReservationService.Endpoints.Auth;

using BusReservationService.Database.Repository;
using BusReservationService.Domain.Entity;
using BusReservationService.Domain.Repository;
using BusReservationService.Domain.ValueObject;
using BusReservationService.Models;
using BusReservationService.Services.Interface;
using FastEndpoints;
using System.Net.NetworkInformation;

public class Activate(ISessionService<Guid, SessionValue> SessionService, IUserRepository UserRepository) : Endpoint<ActivateRequest>
{
    public override void Configure()
    {
        Verbs(Http.POST);
        Routes("/auth/activate");
        Claims("SessionId", "Status");
        AllowAnonymous();
    }

    public override async Task HandleAsync(ActivateRequest req, CancellationToken c)
    {
        var SessionId = HttpContext.Request.Cookies["SessionId"]!;
        Guid id = Guid.Parse(SessionId!);

        var session = SessionService[id];
        if (session is null)
        {
            await Send.ErrorsAsync();
            return;
        }

        var accountId = session.AccountId;

        if (await UserRepository.ExistByAccountId(session.AccountId))
        {
            await Send.ErrorsAsync();
            return;
        }

        var entity = new UserEntity(
            accountId, 
            req.FirstName, 
            req.LastName, 
            req.PhoneNumber, 
            (UserType)Enum.Parse(typeof(UserType), 
            req.UserType.ToString(), true)
            );

        var ret = await UserRepository.AddAsymc(entity);

        await Send.CreatedAtAsync("/auth/activate");
    }
}

public record ActivateRequest(string FirstName, string LastName, string PhoneNumber, string UserType);