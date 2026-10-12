using FastEndpoints;

namespace BusReservationService.Endpoints.Reservation;

internal class UpdateReservation : Endpoint<UpdateReservationRequest>
{
    public override void Configure()
    {
        Verbs(Http.PUT);
        Routes("/api/reservation/{id}");
        Claims("SessionId");
    }

    public override async Task HandleAsync(UpdateReservationRequest req, CancellationToken ct)
    {
        await base.HandleAsync(req, ct);
    }
}

internal record UpdateReservationRequest()
{
    [FromClaim]
    public Guid SessionId { get; set; }
}
internal record UpdateReservationResponse();