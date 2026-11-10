using FastEndpoints;

namespace BusReservationService.Endpoints.Reservation;

internal class DeleteReservation : Endpoint<DeleteReservationRequest>
{
    public override void Configure()
    {
        Verbs(Http.POST);
        Routes("/administrator/reservation/{id}/delete");
        Roles("Admin");
        Claims("AccountId");
    }

    public override async Task HandleAsync(DeleteReservationRequest req, CancellationToken ct)
    {
        await base.HandleAsync(req, ct);
    }
}

internal record DeleteReservationRequest()
{
    [FromClaim]
    public Guid AccountId { get; set; }
}
internal record DeleteReservationResponse();