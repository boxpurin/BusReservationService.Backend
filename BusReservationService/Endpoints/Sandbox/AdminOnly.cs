using FastEndpoints;

namespace BusReservationService.Endpoints.Sandbox;

internal class AdminOnly() : Endpoint<AdminOnlyRequest, AdminOnlyResponse>
{
    public override void Configure()
    {
        Verbs(Http.GET);
        Routes("/api/auth");
        Policies("AdminOnly");
    }

    public override async Task HandleAsync(AdminOnlyRequest req, CancellationToken c)
    {
        await Send.OkAsync(new AdminOnlyResponse("You are an admin."));
    }
}


internal record AdminOnlyRequest();
internal record AdminOnlyResponse(string Message);