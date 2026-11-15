using BusReservationService.Domain.Entity;
using BusReservationService.Domain.Repository;
using BusReservationService.Domain.ValueObject.Prefecture;
using BusReservationService.Domain.ValueObject.AccountRole;
using BusReservationService.Models;
using BusReservationService.Services.Interface;
using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BusReservationService.Endpoints.Administrator.ParkingLots;
    
internal class UpdateParkingLot(IAccountRepository accountRepository, IParkingLotRepository parkingLotRepository) : Endpoint<UpdateParkingLotRequest, UpdateParkingLotResponse>
{
    public override void Configure()
    {
        Verbs(Http.POST);
        Routes("/administrator/parking-lot/{id}");
        Roles("Admin");
        Claims("AccountId");
    }

    public override async Task HandleAsync(UpdateParkingLotRequest req, CancellationToken ct)
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
            req.ParkingLotName,
            req.ZipCode,
            req.Prefecture,
            req.StreetAddress,
            req.PhoneNumber,
            req.FaxNumber,
            req.MaxCapacity,
            req.ReservationCapacity,
            req.IsAllow,
            req.IsActive
        );

        var updated = await parkingLotRepository.UpdateAsync(update);

        await Send.OkAsync(
        new UpdateParkingLotResponse(
            ParkingLotProfile.FromEntity(updated))
        );
    }
}
internal record UpdateParkingLotRequest(
    string ParkingLotName,
    Prefecture Prefecture,
    string ZipCode,
    string StreetAddress,
    string PhoneNumber,
    string? FaxNumber,
    short MaxCapacity,
    short ReservationCapacity,
    bool IsAllow = false,
    bool IsActive = false
)
{
    [FromClaim]
    public Guid AccountId{get; set;}

    [FromRoute]
    public Guid Id{get; set;}
}

internal class UpdateParkingLotValidator : Validator<UpdateParkingLotRequest>
{
    public UpdateParkingLotValidator()
    {
        RuleFor(x => x.AccountId.Version).Equal(7);
        RuleFor(x => x.ParkingLotName).NotEmpty();
        RuleFor(x => x.ZipCode).Length(7);
        RuleFor(x => x.StreetAddress).NotEmpty();
        RuleFor(x => x.PhoneNumber).NotEmpty();
        RuleFor(x => x.MaxCapacity).NotEmpty();
        RuleFor(x => x.ReservationCapacity).NotEmpty();
    }
}
internal record UpdateParkingLotResponse(ParkingLotProfile? ParkingLot);
