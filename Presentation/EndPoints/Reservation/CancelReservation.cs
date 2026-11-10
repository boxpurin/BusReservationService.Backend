using BusReservationService.Domain.Entity;
using BusReservationService.Domain.Repository;
using BusReservationService.Services.Interface;
using BusReservationService.Share;
using BusReservationService.Share.Utils.Error;
using FastEndpoints;
using Microsoft.AspNetCore.Mvc;

namespace BusReservationService.Endpoints.Reservation;

internal class CancelReservation(
    IReservationCommandService reservationCommandService) : Endpoint<CancelReservationRequest>
{
    public override void Configure()
    {
        Verbs(Http.POST);
        Routes("/reservation/cancel/{id}/");
        Roles("User");
        Claims("AccountId");
    }

    public override async Task HandleAsync(CancelReservationRequest req, CancellationToken ct)
    {
        var result = await reservationCommandService.CancelReservationAsync(req.AccountId, req.Id);
        
        if (result is Err<ReservationEntity, Error> err)
        {
            AddError(err.Value.Message, err.Value.ErrorCode);
            await Send.ErrorsAsync(err.Value.StatusCode);
            return;
        }
        
        await Send.OkAsync();
    }
}

internal record CancelReservationRequest()
{
    [FromClaim]
    public Guid AccountId { get; set; }
    [FromRoute]
    public Guid Id { get; init; }
}
internal record CancelReservationResponse();