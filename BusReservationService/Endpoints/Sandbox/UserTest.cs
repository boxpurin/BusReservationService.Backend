using FastEndpoints;

namespace BusReservationService.Endpoints.Sandbox;

internal class UserTest : Endpoint<UserTestRequest, UserTestResponse>
{
    public override void Configure()
    {
        Verbs(Http.GET);
        Routes("/api/user");
        Policies("VerifiedUser");
    }

    public override async Task HandleAsync(UserTestRequest req, CancellationToken c)
    {
        await Send.OkAsync(new UserTestResponse("You are a user."));
    }
}


internal record UserTestRequest(string Req = "Request");

internal record UserTestResponse(string Message);