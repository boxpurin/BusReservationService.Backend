
using BusReservationService.Domain.Entity;
using BusReservationService.Domain.Repository;
using BusReservationService.Models;
using BusReservationService.Services.Interface;
using BusReservationService.Util;

namespace BusReservationService.Services;

public class RegistrationAccountService(IAccountRepository accountRepository) : IRegistrationAccountService
{
    public async Task<AccountModel?> RegistrationAccountAsync(string email, string password)
    {
        var (hash, salt) = Argon2Helper.HashPassword(password);
        var newAccount = AccountEntity.CreateUserRoleAccount(email, hash, salt);

        if(await accountRepository.IsExistByEmailAsync(email))
        {
            return null;
        }
    
        var createdAccount = await accountRepository.AddAsync(newAccount);
        return new AccountModel(createdAccount);
    }
}