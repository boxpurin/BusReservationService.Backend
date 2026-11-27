using BusReservationService.Application.Core.Models;
using BusReservationService.Application.Core.Services.Interface;
using BusReservationService.Share;
using BusReservationService.Share.Utils.Error;
using FastEndpoints;
using FastEndpoints.Security;
using FluentValidation;


internal class GetAvailabilityList(IParkingLotAvailabilityListService parkingLotAvailabilityService) : Endpoint<GetAvailabilityListRequest, GetAvailabilityListResponse>
{
    public override void Configure()
    {
        Get("/parking-lots/{ParkingLotId}/availability");
        Claims("AccountId");
    }

    public override async Task HandleAsync(GetAvailabilityListRequest req, CancellationToken ct)
    {
        // Implement the logic to get parking lot availability here
        var availability = await parkingLotAvailabilityService.GetDailyAvailabilityAsync(req.ParkingLotId, DateOnly.FromDateTime(req.DateTimeAt));

        if(availability is Err<ParkingLotAvailability, Error> (var e) )
        {
            AddError(e.Message, e.ErrorCode);
            await Send.ErrorsAsync(e.StatusCode, cancellation: ct);
            return;
        }

        await Send.OkAsync(
            new GetAvailabilityListResponse(availability.Unwrap()), cancellation: ct
        );
    }
}

internal record GetAvailabilityListRequest
{
    [FromClaim]
    public Guid AccountId { get; init; }

    [RouteParam]
    public Guid ParkingLotId { get; init; }

    [FromQuery]
    public DateTime DateTimeAt { get; init; }
}

internal class GetAvailabilityListRequestValidator : Validator<GetAvailabilityListRequest>
{
    public GetAvailabilityListRequestValidator()
    {
        RuleFor(x => x.AccountId)
        .NotEmpty();

        RuleFor(x => x.AccountId.Version)
        .Equal(7);

        RuleFor(
            x => x.ParkingLotId
        ).NotEmpty();

        RuleFor(x => x.DateTimeAt)
        .NotEmpty()
        .Must(date => date >= DateTime.Today);
    }
}


internal record GetAvailabilityListResponse(ParkingLotAvailability availability);