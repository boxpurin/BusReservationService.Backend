using BusReservationService.Domain.Entity;
using BusReservationService.Domain.Repository;
using BusReservationService.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusReservationService.Domain.DomainService;
public class CreateAccountService 
{
    readonly IAccountRepository AccountRepository;
    readonly IUserRepository UserRepository;

    CreateAccountService(IAccountRepository accountRepository, IUserRepository userRepository)
    {
        AccountRepository = accountRepository;
        UserRepository = userRepository;
    }

    public async Task CreateNewAccount(string email, string password)
    {
        var account = new AccountEntity(
            Guid.CreateVersion7(),
            email,
            password,
            AccountRole.User,
            AccountStatus.JustCreated
        );
    }
}