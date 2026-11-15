using BusReservationService.Domain.Repository;
using BusReservationService.Domain.ValueObject.AccountRole;
using BusReservationService.Domain.ValueObject.Prefecture;
using BusReservationService.Domain.ValueObject.UserType;
using BusReservationService.Models;
using BusReservationService.Services.Interface;
using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BusReservationService.Endpoints.Administrator.ParkingLots;

internal class AddParkingLot(IAccountRepository accountRepository, IParkingLotRepository parkingLotRepository) : Endpoint<AddParkingLotRequest, AddParkingLotResponse>
{
    public override void Configure()
    {
        Verbs(Http.POST);
        Routes("/administrator/parking-lot");
        Roles("Admin");
        Claims("AccountId");
    }

    public override async Task HandleAsync(AddParkingLotRequest req, CancellationToken ct)
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

        var dto = new AddParkingLotDto(
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

        var lot = await parkingLotRepository.AddAsync(dto);

        await Send.CreatedAtAsync($"api/parking-lot/{lot.Id}",
        new AddParkingLotResponse(
            ParkingLotProfile.FromEntity(lot))
        );
    }
}
internal record AddParkingLotRequest(
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
}

internal class AddParkingLotValidator : Validator<AddParkingLotRequest>
{
    public AddParkingLotValidator()
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
internal record AddParkingLotResponse(ParkingLotProfile? ParkingLot);
