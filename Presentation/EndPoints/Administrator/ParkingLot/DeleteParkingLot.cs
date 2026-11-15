using System.Reflection.Metadata.Ecma335;
using BusReservationService.Domain.Repository;
using BusReservationService.Domain.ValueObject.AccountRole;
using BusReservationService.Models;
using BusReservationService.Services.Interface;
using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BusReservationService.Endpoints.Administrator.ParkingLots;
    
internal class DeleteParkingLot(IAccountRepository accountRepository, IParkingLotRepository parkingLotRepository) : Endpoint<DeleteParkingLotRequest, DeleteParkingLotResponse>
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
        
        var account = await accountRepository.FindByAccountIdAsync(req.AccountId);
        if( account is null )
        {
            AddError("Account not found.");
            await Send.NotFoundAsync();
            return;
        }

        if( account.AccountRole != AccountRole.Admin )
        {
            AddError("Forbidden access. Admin role required.");
            await Send.ForbiddenAsync();
            return;
        }

        if(!await parkingLotRepository.ExistByParkingLotIdAsync(req.Id))
        {
            AddError("Parking lot not found.");
            await Send.NotFoundAsync();
            return;
        }

        await parkingLotRepository.DeleteAsync(req.Id);
        await Send.NoContentAsync();
        return;
    }
}


internal record DeleteParkingLotRequest()
{
    [FromRoute]
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
