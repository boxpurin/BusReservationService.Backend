using FastEndpoints;

namespace BusReservationService.Endpoints.Reservation;

internal class CancelReservation : Endpoint<CancelReservationRequest>
{
    public override void Configure()
    {
        Verbs(Http.POST);
        Routes("/api/reservation/{id}/cancel");
        Claims("SessionId");
    }

    public override async Task HandleAsync(CancelReservationRequest req, CancellationToken ct)
    {
        await base.HandleAsync(req, ct);
    }
}

internal record CancelReservationRequest()
{
    [FromClaim]
    public Guid SessionId { get; set; }
}
internal record CancelReservationResponse();