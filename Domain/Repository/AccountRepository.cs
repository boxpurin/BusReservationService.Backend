using BusReservationService.Domain.Entity;

namespace BusReservationService.Domain.Repository;

public interface IAccountRepository
{
    Task<AccountEntity> AddAsync(AccountEntity account);

    Task<AccountEntity> UpdateAsync(AccountEntity account);

    Task<AccountEntity[]> GetAllAsync();
    Task<AccountEntity?> FindByAccountId(Guid accountId);
    Task<AccountEntity?> FindByEmail(string email);

    Task<bool> IsExistByAccountId(Guid accountId);
    Task<bool> IsExistByEmail(string email);

    Task<bool> DeleteAsync(AccountEntity account);
}