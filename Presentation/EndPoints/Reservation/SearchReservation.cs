using BusReservationService.Domain.Entity;
using BusReservationService.Domain.Repository;
using FastEndpoints;

namespace BusReservationService.Endpoints.Reservation;

internal class SearchReservation(IReservationRepository reservationRepository) : Endpoint<SearchReservationRequest, SearchReservationResponse>
{
    public override void Configure()
    {
        Verbs(Http.GET);
        Routes("/reservation");
        Roles("User");
        Claims("AccountId");
    }

    public override async Task HandleAsync(SearchReservationRequest req, CancellationToken ct)
    {
        var reservations = await reservationRepository.FindByAccountIdAsync(req.AccountId);

        
        var results = reservations.Where(
            x => x.ParkingLotId == req.ParkingLotId
        );

        await Send.OkAsync(new SearchReservationResponse(results.ToList()));
    }
}

internal record SearchReservationRequest()
{
    [FromClaim]
    public Guid AccountId { get; init; }
    [FromQuery]
    public Guid ParkingLotId { get; init; }
}
internal record SearchReservationResponse(List<ReservationEntity> Reservations);