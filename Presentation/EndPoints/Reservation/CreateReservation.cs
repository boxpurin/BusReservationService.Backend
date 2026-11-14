using BusReservationService.Domain.Entity;
using BusReservationService.Domain.Repository;
using BusReservationService.Services.Interface;
using BusReservationService.Share;
using BusReservationService.Share.Utils.Error;
using FastEndpoints;
using FluentValidation;

namespace BusReservationService.Endpoints.Reservation;

internal class CreateReservation(
    IAccountRepository accountRepository,
    IReservationCommandService reservationCommandService) : Endpoint<CreateReservationRequest>
{
    public override void Configure()
    {
        Verbs(Http.POST);
        Routes("/reservation");
        Roles("User");
        Claims("AccountId");
    }

    public override async Task HandleAsync(CreateReservationRequest req, CancellationToken ct)
    {
        var account = await accountRepository.FindByAccountIdAsync(req.AccountId);
        
        if (account is null)
        {
            AddError("アカウントが存在しません");
            await Send.ForbiddenAsync();
            return;
        }

        if (account.AccountStatus != Domain.ValueObject.AccountStatus.AccountStatus.Activated)
        {
            AddError("アクティブ化されていないアカウントです");
            await Send.ForbiddenAsync();
            return;
        }

        var result = await reservationCommandService.CreateReservationAsync(
            req.AccountId,
            req.ParkingLotId,
            req.IncomingDateTime,
            req.OutgoingDateTime,
            req.BusCampanyName
        );

        if (result is Err<ReservationEntity, Error> err)
        {
            AddError(err.Value.Message, err.Value.ErrorCode);
            await Send.ErrorsAsync(err.Value.StatusCode);
            return;
        }

        var reservation = result.Unwrap();
        await Send.OkAsync(new CreateReservationResponse(reservation));
    }
}

internal record CreateReservationRequest(
    Guid ParkingLotId,
    DateTime IncomingDateTime,
    DateTime OutgoingDateTime,
    string BusCampanyName)
{
    [FromClaim]
    public Guid AccountId { get; init; }
}

internal class CreateReservationRequestValidator : Validator<CreateReservationRequest>
{
    public CreateReservationRequestValidator()
    {
        RuleFor(x => x.ParkingLotId).NotEmpty();
        RuleFor(x => x.IncomingDateTime).
            LessThan(x => x.OutgoingDateTime).
            WithMessage("IncomingDateTime must be less than OutgoingDateTime");
        RuleFor(x => x.BusCampanyName).NotEmpty();
    }
}

internal record CreateReservationResponse(ReservationEntity Reservation);