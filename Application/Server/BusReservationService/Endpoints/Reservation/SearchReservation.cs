using FastEndpoints;

namespace BusReservationService.Endpoints.Reservation;

internal class SearchReservation : Endpoint<SearchReservationRequest>
{
    public override void Configure()
    {
        Verbs(Http.GET);
        Routes("/api/reservation");
        Claims("SessionId");
    }

    public override async Task HandleAsync(SearchReservationRequest req, CancellationToken ct)
    {
        await base.HandleAsync(req, ct);
    }
}

internal record SearchReservationRequest()
{
    [FromClaim]
    public Guid SessionId { get; set; }
}
internal record SearchReservationResponse();