using BusReservationService.Domain.Entity;
using BusReservationService.Domain.Repository;
using BusReservationService.Models;
using FastEndpoints;
using Microsoft.AspNetCore.Mvc;

namespace BusReservationService.Endpoints.Reservation;

internal class GetReservation(IReservationRepository reservationRepository) : Endpoint<GetReservationRequest>
{
    public override void Configure()
    {
        Verbs(Http.GET);
        Routes("/reservation/{id}");
        Roles("User");
        Claims("AccountId");
    }

    public override async Task HandleAsync(GetReservationRequest req, CancellationToken ct)
    {
        var reservation = await reservationRepository.FindByReservationIdAsync(req.Id);
        
        if(reservation is null)
        {
            AddError("予約情報が見つかりません", "ReservationNotFound" );
            await Send.NotFoundAsync();
            return;
        }

        if (reservation.AccountId != req.AccountId)
        {
            AddError("予約情報へのアクセス権限がありません", "AccessDenied" );
            await Send.ForbiddenAsync();
            return;
        }

        await Send.OkAsync(new GetReservationResponse(reservation));
    }
}

internal record GetReservationRequest()
{
    [FromClaim]
    public Guid AccountId { get; init; }

    [FromRoute]
    public Guid Id { get; init;}
}
internal record GetReservationResponse(ReservationEntity Reservation);