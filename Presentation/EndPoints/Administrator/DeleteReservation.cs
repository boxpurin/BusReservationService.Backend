using Application.Core.Services.Interface.Administrator;
using BusReservationService.Domain.Repository;
using BusReservationService.Domain.ValueObject.AccountRole;
using BusReservationService.Share;
using BusReservationService.Share.Utils.Error;
using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace BusReservationService.Endpoints.Reservation;

internal class DeleteReservation(
    IAdministratorReservationCommandService administratorReservationCommandService) : Endpoint<DeleteReservationRequest>
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
        var result = await administratorReservationCommandService.DeleteReservationAsync(
            req.AccountId, req.ReservationId
        );

        if (result is Err<int, Error>  (var err))
        {
            AddError(err.Message, err.ErrorCode);
            await Send.ErrorsAsync(err.StatusCode, ct);
            return;
        }

        await Send.NoContentAsync(ct);
    }
}

internal record DeleteReservationRequest()
{
    [BindFrom("id")]
    public Guid ReservationId { get; set; }

    [FromClaim]
    public Guid AccountId { get; set; }
}

internal class DeleteReservationRequestValidator : Validator<DeleteReservationRequest>
{
    public DeleteReservationRequestValidator()
    {
        RuleFor(x => x.ReservationId).NotEqual(Guid.Empty);
        RuleFor(x => x.ReservationId.Version).Equal(7);

        RuleFor(x => x.AccountId).NotEqual(Guid.Empty);
        RuleFor(x => x.AccountId.Version).Equal(7);
    }
}

internal record DeleteReservationResponse();