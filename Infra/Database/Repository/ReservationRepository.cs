
using BusReservationService.Domain.Entity;
using BusReservationService.Domain.Repository;
using BusReservationService.Share;
using System.Data;

namespace BusReservationService.Database.Repository;

public class ReservationRepository : IReservationRepository
{
    private readonly IDbConnection _connection;

    public ReservationRepository(IDbConnection dbConnection)
    {
        _connection = dbConnection;
    }

    public async Task<ReservationEntity> AddAsync(ReservationEntity reservation)
    {
        throw new NotImplementedException();
    }
    public async Task<ReservationEntity> UpdateAsync(ReservationEntity reservation)
    {
        throw new NotImplementedException();
    }
    public async Task<ReservationEntity?> FindByReservationIdAsync(Guid reservationId)
    {
        throw new NotImplementedException();
    }
    public async Task<ReservationEntity[]> FindByAccountIdAsync(Guid accountId)
    {
        throw new NotImplementedException();
    }

    public async Task<ReservationEntity[]> FindByParkingLotIdAsync(Guid parkingLotId)
    {
        throw new NotImplementedException();
    }
    public async Task<bool> ExistByAccountIdAsync(Guid accountId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool?> ExistByReservationIdAsync(Guid reservationId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteByReservationIdAsync(Guid reservationId)
    {
        throw new NotImplementedException();
    }

}
