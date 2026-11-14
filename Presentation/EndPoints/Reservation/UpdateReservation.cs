using BusReservationService.Domain.Entity;
using BusReservationService.Domain.Repository;
using BusReservationService.Services.Interface;
using BusReservationService.Share;
using BusReservationService.Share.Utils.Error;
using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace BusReservationService.Endpoints.Reservation;

internal class UpdateReservation(IReservationCommandService reservationCommandService) : Endpoint<UpdateReservationRequest>
{
    public override void Configure()
    {
        Verbs(Http.PUT);
        Routes("/reservation/{id}");
        Roles("User");
        Claims("AccountId");
    }

    public override async Task HandleAsync(UpdateReservationRequest req, CancellationToken ct)
    {
        var result = await reservationCommandService.UpdateReservationAsync(
            req.AccountId,
            req.Id,
            req.IncomingDateTime,
            req.OutgoingDateTime,
            req.BusCampanyName);

        if (result is Err<ReservationEntity, Error> err)
        {
            AddError(err.Value.Message, err.Value.ErrorCode);
            await Send.ErrorsAsync((int)err.Value.StatusCode);
            return;
        }

        await Send.OkAsync();
    }
}

internal record UpdateReservationRequest(
    Guid ParkingLotId,
    DateTime IncomingDateTime,
    DateTime OutgoingDateTime,
    string BusCampanyName)
{
    [FromClaim]
    public Guid AccountId { get; set; }
    [FromRoute]
    public Guid Id { get; set; }
}

internal class UpdateReservationRequestValidator : Validator<UpdateReservationRequest>
{
    public UpdateReservationRequestValidator()
    {
        RuleFor(x => x.ParkingLotId).NotEmpty();
        RuleFor(x => x.IncomingDateTime).
            LessThan(x => x.OutgoingDateTime).
            WithMessage("IncomingDateTime must be less than OutgoingDateTime");
        RuleFor(x => x.BusCampanyName).NotEmpty();
    }
}

internal record UpdateReservationResponse();