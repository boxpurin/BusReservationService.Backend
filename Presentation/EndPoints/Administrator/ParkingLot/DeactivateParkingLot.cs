using System.Reflection.Metadata.Ecma335;
using BusReservationService.Domain.Entity;
using BusReservationService.Domain.Repository;
using BusReservationService.Domain.ValueObject.UserType;
using BusReservationService.Domain.ValueObject.AccountRole;
using BusReservationService.Models;
using BusReservationService.Services.Interface;
using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BusReservationService.Endpoints.Administrator.ParkingLots;

internal class DeactiveParkingLot(IAccountRepository accountRepository,IParkingLotRepository parkingLotRepository) : Endpoint<DeactiveParkingLotRequest, DeactiveParkingLotResponse>
{
    public override void Configure()
    {
        Verbs(Http.POST);
        Routes("/administrator/parking-lot/deactivate/{id}");
        Roles("Admin");
        Claims("AccountId");
    }

    public override async Task HandleAsync(DeactiveParkingLotRequest req, CancellationToken ct)
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
        var lot = await parkingLotRepository.FindByParkingLotIdAsync(req.Id);

        if(lot is null)
        {
            AddError("Parking lot not found.");
            await Send.NotFoundAsync();
            return;
        }

        var update = ParkingLotEntity.FromSnapshot(
            lot.Id,
            lot.ParkingLotName,
            lot.ZipCode,
            lot.Prefecture,
            lot.StreetAddress,
            lot.PhoneNumber,
            lot.FaxNumber,
            lot.MaxCapacity,
            lot.ReservationCapacity,
            lot.IsAllowsBoardingAlighting,
            false
        );

        var updated = await parkingLotRepository.UpdateAsync(update);

        await Send.OkAsync(new DeactiveParkingLotResponse(
            ParkingLotProfile.FromEntity(updated))
        );
        return;
    }
}


internal record DeactiveParkingLotRequest()
{
    [FromRoute]
    public Guid Id {get; init;}

    [FromClaim]
    public Guid AccountId {get; init;}
}

internal class DeactiveParkingLotRequestValidator : Validator<DeactiveParkingLotRequest>
{
    public DeactiveParkingLotRequestValidator()
    {
        RuleFor(x => x.Id.Version).Equal(7);
    }
}


internal record DeactiveParkingLotResponse(ParkingLotProfile UpdatedModel);
