using FastEndpoints;

namespace BusReservationService.Endpoints.Reservation;

internal class GetReservation : Endpoint<GetReservationRequest>
{
    public override void Configure()
    {
        Verbs(Http.GET);
        Routes("/api/reservation/{id}");
        Claims("SessionId");
    }

    public override async Task HandleAsync(GetReservationRequest req, CancellationToken ct)
    {
        await base.HandleAsync(req, ct);
    }
}

internal record GetReservationRequest()
{
    [FromClaim]
    public Guid SessionId { get; set; }
}
internal record GetReservationResponse();