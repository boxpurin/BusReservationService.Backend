
using BusReservationService.Domain.Entity;
using BusReservationService.Domain.Repository;
using BusReservationService.Models;
using BusReservationService.Services.Interface;
using BusReservationService.Util;
using BusReservationService.Share;
using BusReservationService.Share.Utils.Error;
using System.Data;
using Dapper;
using BusReservationService.Database.Interface;

namespace BusReservationService.Infra.Database.Service;

public class RegistrationAccountService(IDbConnection conn, IAccountRepository accountRepository) : IConnection, IRegistrationAccountService
{
    public async Task<ApplicationResult<AccountModel, Error>> RegistrationAccountAsync(string email, string password)
    {
        EnsureConnectionOpen(conn);

        if(await accountRepository.IsExistByEmailAsync(email))
        {
            return new Err<AccountModel, Error>(
                new Error {
                    Message = "The email is already registered.",
                    ErrorCode = "EmailAlreadyExists",
                    StatusCode = 409
                }
            );
        }
        
        var (hash, salt) = Argon2Helper.HashPassword(password);
        var newAccount = AccountEntity.CreateUserRoleAccount(email, hash, salt);
        
        newAccount.WaitForOnboard();
        
        using var tx = conn.BeginTransaction(IsolationLevel.ReadCommitted);
        
        try
        {
            var createdAccount = await accountRepository.AddAsync(newAccount);
            tx.Commit();

            return new Ok<AccountModel, Error>(new AccountModel(createdAccount));
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }
}