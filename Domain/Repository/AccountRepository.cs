using BusReservationService.Domain.Entity;

namespace BusReservationService.Domain.Repository;

public interface IAccountRepository
{
    Task<AccountEntity> AddAsync(AccountEntity account);

    Task<AccountEntity> UpdateAsync(AccountEntity account);

    Task<AccountEntity[]> GetAllAsync();
    Task<AccountEntity?> FindByAccountIdAsync(Guid accountId);
    Task<AccountEntity?> FindByEmailAsync(string email);

    Task<bool> IsExistByAccountIdAsync(Guid accountId);
    Task<bool> IsExistByEmailAsync(string email);

    Task<bool> DeleteAsync(AccountEntity account);
}