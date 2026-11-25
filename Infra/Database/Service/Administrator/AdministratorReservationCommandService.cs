using System.Data;
using Dapper;
using Application.Core.Services.Interface.Administrator;
using BusReservationService.Database.Interface;
using BusReservationService.Database.Table;
using BusReservationService.Domain.Repository;
using BusReservationService.Domain.ValueObject.AccountRole;
using BusReservationService.Share;
using BusReservationService.Share.Utils.Error;
using BusReservationService.Infra.Database;
using BusReservationService;

public class AdministratorReservationCommandService(
    IReservationRepository reservationRepository,
    IAccountRepository accountRoleRepository,
    IUnitOfWork unit) : IAdministratorReservationCommandService
{

    public async Task<ApplicationResult<int, Error>> DeleteReservationAsync(Guid adminAccountId, Guid reservationId)
    {

        await unit.BeginAsync();
        var adminAccount = await accountRoleRepository.FindByAccountIdAsync(adminAccountId);

        if (adminAccount is null)
        {
            return new Err<int, Error>(new Error{
                Message = "Unauthorized",
                ErrorCode = "UNAUTHORIZED_ACCOUNT",
                StatusCode = 401 }
            );
        }

        if (adminAccount.AccountRole != AccountRole.Admin)
        {
            return new Err<int, Error>(new Error{
                Message = "Forbidden",
                ErrorCode = "FORBIDDEN",
                StatusCode = 403 }
            );
        }

        try
        {
            var lockedReservation = await reservationRepository.FindByReservationIdForUpdateAsync(reservationId);

            if (lockedReservation is null)
            {
                await unit.RollbackAsync();
                return new Err<int, Error>(new Error{
                    Message = "Reservation not found",
                    ErrorCode = "RESERVATION_NOT_FOUND",
                    StatusCode = 404 }
                );
            }

            var result = await reservationRepository.DeleteByReservationIdAsync(lockedReservation.Id);

            await unit.CommitAsync();

            return new Ok<int, Error>(result ? 1 : 0);
        }
        catch
        {
            await unit.RollbackAsync();
            throw;
        }
    }
}