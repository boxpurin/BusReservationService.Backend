using BusReservationService.Domain.Entity;
using BusReservationService.Share;

namespace BusReservationService.Domain.Repository;

public interface IAccountRepository
{
    Task<AccountEntity> AddAsync(AccountEntity account);
    Task<AccountEntity[]> AddListAsync(AccountEntity[] account);

    Task<AccountEntity> UpdateAsync(AccountEntity account);
    Task<AccountEntity[]> UpdateListAsync(AccountEntity[] account);

    Task<AccountEntity[]> GetAllAsync();
    Task<AccountEntity?> FindByAccountId(Guid accountId);
    Task<AccountEntity?> FindByEmail(string email);

    Task<bool> IsExistByAccountId(Guid accountId);
    Task<bool> IsExistByEmail(string email);

    Task<bool> DeleteListAsync(AccountEntity[] account);
    Task<bool> DeleteAsync(AccountEntity account);
}