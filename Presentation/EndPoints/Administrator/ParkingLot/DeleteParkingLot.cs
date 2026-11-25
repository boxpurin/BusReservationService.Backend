using System.Reflection.Metadata.Ecma335;
using Application.Core.Services.Interface.Administrator;
using BusReservationService.Domain.Repository;
using BusReservationService.Domain.ValueObject.AccountRole;
using BusReservationService.Models;
using BusReservationService.Services.Interface;
using BusReservationService.Share;
using BusReservationService.Share.Utils.Error;
using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BusReservationService.Endpoints.Administrator.ParkingLots;
    
internal class DeleteParkingLot(IAdministratorReservationCommandService administratorReservationCommandService) : Endpoint<DeleteParkingLotRequest, DeleteParkingLotResponse>
{
    public override void Configure()
    {
        Verbs(Http.POST);
        Routes("/administrator/parking-lot/delete/{id}");
        Roles("Admin");
        Claims("AccountId");
    }

    public override async Task HandleAsync(DeleteParkingLotRequest req, CancellationToken ct)
    {
        var result = await administratorReservationCommandService.DeleteReservationAsync(req.AccountId, req.Id);
        if(result is Err<int, Error> (var err))
        {
            AddError(err.Message, err.ErrorCode);
            await Send.ErrorsAsync(err.StatusCode);
            return ;
        }

        await Send.NoContentAsync();
    }
}


internal record DeleteParkingLotRequest()
{
    [RouteParam]
    public Guid Id {get; init;}

    [FromClaim]
    public Guid AccountId{ get; init;}
}

internal class DeleteParkingLotRequestValidator : Validator<DeleteParkingLotRequest>
{
    public DeleteParkingLotRequestValidator()
    {
        RuleFor(x => x.Id.Version).Equal(7);
    }
}


internal record DeleteParkingLotResponse();
