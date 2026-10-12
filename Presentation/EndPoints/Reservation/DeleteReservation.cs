using FastEndpoints;

namespace BusReservationService.Endpoints.Reservation;

internal class DeleteReservation : Endpoint<DeleteReservationRequest>
{
    public override void Configure()
    {
        Verbs(Http.POST);
        Routes("/api/reservation/{id}/delete");
        Claims("SessionId");
    }

    public override async Task HandleAsync(DeleteReservationRequest req, CancellationToken ct)
    {
        await base.HandleAsync(req, ct);
    }
}

internal record DeleteReservationRequest()
{
    [FromClaim]
    public Guid SessionId { get; set; }
}
internal record DeleteReservationResponse();