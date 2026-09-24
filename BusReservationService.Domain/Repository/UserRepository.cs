using BusReservationService.Domain.Entity;
using BusReservationService.Share;

namespace BusReservationService.Domain.Repository;

public interface IUserRepository
{
    Task<UserEntity> AddAsymc(UserEntity entity);
    Task<UserEntity[]> AddListAsync(UserEntity[] entity);
    Task<UserEntity> UpdateAsync(UserEntity entity);
    Task<UserEntity[]> UpdateListAsync(UserEntity[] entity);
    Task<UserEntity?> FindByAccountId(Guid accountId);
    Task<bool> ExistByAccountId(Guid accountId);
    Task<bool> DeleteById(Guid accountId);
}
