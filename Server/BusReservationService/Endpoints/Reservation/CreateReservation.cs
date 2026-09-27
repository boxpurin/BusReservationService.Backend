using FastEndpoints;

namespace BusReservationService.Endpoints.Reservation;

internal class CreateReservation : Endpoint<CreateReservationRequest>
{
    public override void Configure()
    {
        Verbs(Http.POST);
        Routes("/api/reservation");
        Claims("SessionId");
    }

    public override async Task HandleAsync(CreateReservationRequest req, CancellationToken ct)
    {
        await base.HandleAsync(req, ct);
    }
}

internal record CreateReservationRequest()
{
    [FromClaim]
    public Guid SessionId { get; set; }
}
internal record CreateReservationResponse();