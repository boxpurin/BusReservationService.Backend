using BusReservationService.Database.Repository;
using BusReservationService.Database.Table;
using BusReservationService.Domain.Entity;
using BusReservationService.Domain.Repository;
using BusReservationService.Domain.ValueObject;
using BusReservationService.Dto.Request;
using BusReservationService.Services.Interface;
using BusReservationService.Share;
using System.Data;

namespace BusReservationService.Services;
public class AuthService(IAccountRepository AccountRepository, UserRepository UserRepository) : IAccountService
{
    public async Task<AccountEntity?> RegistorationAccount(RegistrationAccount dto)
    {
        AccountEntity entity = AccountEntity.CreateUserRoleAccount(dto.Email, dto.Password);

        var result = await AccountRepository.AddAsync(entity);

        return result;
    }

    public async Task<AccountEntity?> SinginAccount(string email, string password)
    {
        var account = await AccountRepository.FindByEmail(email);

        if (account is null)
        {
            return null;
        }

        if(account.Password != password)
        {
            return null;
        }

        await AccountRepository.UpdateAsync(account);

        return account;
    }

    public async Task<AccountEntity?> SuspendAccountAsync(Guid accountId)
    {
        var account = await AccountRepository.FindByAccountId(accountId);
        if (account is null)
        {
            return null;
        }

        if(!account.AccountStatus.IsSuspend())
        {
            account.Suspend();
            await AccountRepository.UpdateAsync(account);
        }

        return account;
    }
}
