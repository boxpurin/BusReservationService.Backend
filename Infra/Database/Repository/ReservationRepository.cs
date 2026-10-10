using BusReservationService.Database.Interface;
using BusReservationService.Domain.Entity;
using BusReservationService.Domain.Repository;
using BusReservationService.Share;
using System.Data;

namespace BusReservationService.Database.Repository;

public class ReservationRepository(IDbConnection connection) : IConnection, IReservationRepository
{

    public async Task<ReservationEntity> AddAsync(ReservationEntity reservation)
    {
        EnsureConnectionOpen(connection);

        throw new NotImplementedException();
    }
    public async Task<ReservationEntity> UpdateAsync(ReservationEntity reservation)
    {
        EnsureConnectionOpen(connection);

        throw new NotImplementedException();
    }
    public async Task<ReservationEntity?> FindByReservationIdAsync(Guid reservationId)
    {
        EnsureConnectionOpen(connection);

        throw new NotImplementedException();
    }
    public async Task<ReservationEntity[]> FindByAccountIdAsync(Guid accountId)
    {
        EnsureConnectionOpen(connection);

        throw new NotImplementedException();
    }

    public async Task<ReservationEntity[]> FindByParkingLotIdAsync(Guid parkingLotId)
    {
        EnsureConnectionOpen(connection);

        throw new NotImplementedException();
    }
    public async Task<bool> ExistByAccountIdAsync(Guid accountId)
    {
        EnsureConnectionOpen(connection);

        throw new NotImplementedException();
    }

    public async Task<bool?> ExistByReservationIdAsync(Guid reservationId)
    {
        EnsureConnectionOpen(connection);

        throw new NotImplementedException();
    }

    public async Task<bool> DeleteByReservationIdAsync(Guid reservationId)
    {
        EnsureConnectionOpen(connection);

        throw new NotImplementedException();
    }

}
