using BusReservationService.Domain.ValueObject.AccountRole;
using BusReservationService.Domain.ValueObject.AccountStatus;
using BusReservationService.Domain.ValueObject.UserType;
using BusReservationService.Services.Interface;
using FastEndpoints;

namespace BusReservationService.Endpoints.User;

internal class GetUserProfile(IAccountUserQueryService accountUserQuery) : Endpoint<GetUserProfileRequest, GetUserProfileResponse>
{
    public override void Configure()
    {
        Verbs(Http.GET);
        Routes("/user/profile");
        Claims("AccountId");
        Roles("User");
    }

    public override async Task HandleAsync(GetUserProfileRequest req, CancellationToken c)
    {
        var accountUserResult = await accountUserQuery.GetModelByAccountIdAsync(req.AccountId);
        if (accountUserResult is null)
        {
            await Send.NotFoundAsync();
            return;
        }

        await Send.OkAsync(new GetUserProfileResponse(new UserProfile(accountUserResult)));
    }
}

internal record GetUserProfileRequest
{
    [FromClaim]
    public Guid AccountId { get; set; } = Guid.Empty;
}

internal record GetUserProfileResponse(UserProfile UserProfile);