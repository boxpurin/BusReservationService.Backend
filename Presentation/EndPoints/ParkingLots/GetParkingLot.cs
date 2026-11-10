using BusReservationService.Domain.Repository;
using BusReservationService.Domain.ValueObject.UserType;
using BusReservationService.Models;
using BusReservationService.Services.Interface;
using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ResponseType = Microsoft.AspNetCore.Http.HttpResults.Results<
    Microsoft.AspNetCore.Http.HttpResults.Ok<BusReservationService.Endpoints.ParkingLots.GetParkingLotResponse>,
    Microsoft.AspNetCore.Http.HttpResults.NotFound,
    Microsoft.AspNetCore.Http.HttpResults.InternalServerError
>;

namespace BusReservationService.Endpoints.ParkingLots;

internal class GetParkingLot(IParkingLotRepository parkingLotRepository) : Endpoint<GetParkingLotRequest, ResponseType>
{
    public override void Configure()
    {
        Verbs(Http.GET);
        Routes("/parking-lot/{id}");
        Roles("User");
        Claims("AccountId");
    }

    public override async Task<ResponseType> ExecuteAsync(GetParkingLotRequest req, CancellationToken ct)
    {
        var lot = await parkingLotRepository.FindByParkingLotIdAsync(req.Id);
        if(lot is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(new GetParkingLotResponse(
            ParkingLotProfile.FromEntity(lot))
        );
    }
}
internal record GetParkingLotRequest()
{
    [FromRoute]
    public Guid Id {get; init;}
}

internal class GetParkingLotValidator : Validator<GetParkingLotRequest>
{
    public GetParkingLotValidator()
    {
        RuleFor(x => x.Id.Version).Equal(7);
    }
}
internal record GetParkingLotResponse(ParkingLotProfile? ParkingLot);
