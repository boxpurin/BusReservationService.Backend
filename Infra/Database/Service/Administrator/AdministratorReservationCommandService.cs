using System.Data;
using Dapper;
using Application.Core.Services.Interface.Administrator;
using BusReservationService.Database.Interface;
using BusReservationService.Database.Table;
using BusReservationService.Domain.Repository;
using BusReservationService.Domain.ValueObject.AccountRole;
using BusReservationService.Share;
using BusReservationService.Share.Utils.Error;

public class AdministratorReservationCommandService : IConnection, IAdministratorReservationCommandService
{
    private readonly IAccountRepository accountRoleRepository;
    private readonly IReservationRepository reservationRepository;
    private readonly IDbConnection conn;

    public AdministratorReservationCommandService(
        IReservationRepository reservationRepository,
        IAccountRepository accountRoleRepository,
        IDbConnection connection)
    {
        this.reservationRepository = reservationRepository;
        this.accountRoleRepository = accountRoleRepository;
        this.conn = connection;
    }

    public async Task<ApplicationResult<int, Error>> DeleteReservationAsync(Guid adminAccountId, Guid reservationId)
    {
        EnsureConnectionOpen(conn);

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

        using var tx = conn.BeginTransaction();

        try
        {
            var lockedReservation = await LockReservationAsync(reservationId);

            if (lockedReservation is null)
            {
                tx.Rollback();
                return new Err<int, Error>(new Error{
                    Message = "Reservation not found",
                    ErrorCode = "RESERVATION_NOT_FOUND",
                    StatusCode = 404 }
                );
            }

            var result = await reservationRepository.DeleteByReservationIdAsync(lockedReservation.Id);

            tx.Commit();

            return new Ok<int, Error>(result ? 1 : 0);
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    private async Task<ReservationTable?> LockReservationAsync(Guid reservationId)
    {
        var sql = "SELECT * FROM Reservation WHERE Id = @ReservationId FOR UPDATE";

        var reservation = await conn.QuerySingleOrDefaultAsync<ReservationTable>(
            sql, new { ReservationId = reservationId }
        );

        return reservation;
    }
}