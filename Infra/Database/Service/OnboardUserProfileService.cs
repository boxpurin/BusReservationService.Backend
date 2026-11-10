

using System.Data;
using Dapper;
using BusReservationService.Database.Interface;
using BusReservationService.Domain.Repository;
using BusReservationService.Domain.ValueObject.UserType;
using BusReservationService.Share;
using BusReservationService.Share.Utils.Error;
using BusReservationService.Models;
using BusReservationService.Services.Interface;
using BusReservationService.Database.Table;

namespace BusReservationService.Infra.Database.Service;

public class OnboardUserProfileService(IDbConnection conn, IAccountRepository accountRepository, IUserRepository userRepository) 
: IConnection, IOnboardUserProfileService
{
    public async Task<ApplicationResult<AccountModel, Error>> OnboardUserProfile(Guid accountId, string firstName, string lastName, string zipCode, string streetAddress, string phoneNumber, string? faxNumber, UserType userType)
    {
        EnsureConnectionOpen(conn);

        using var tx = conn.BeginTransaction(IsolationLevel.ReadCommitted);

        try
        {
            // アカウント情報を習得
            var account = await LockAccountAsync(accountId);
            if (account is null)
            {
                tx.Rollback();
                return new Err<AccountModel, Error>(new Error{
                    Message = "The account was not found.",
                    ErrorCode = "AccountNotFound",
                    StatusCode = 404
                });
            }

            // すでにユーザー情報が追加されているアカウントIDなのかを確認する
            if (await userRepository.ExistByAccountIdAsync(account.Id))
            {
                tx.Rollback();
                return new Err<AccountModel, Error>(new Error{
                    Message = "The user profile already exists for this account.",
                    ErrorCode = "UserAlreadyExists",
                    StatusCode = 409
                });
            }
        
            // ユーザーの登録
            var user = await userRepository.AddAsync(new AddAsyncDto(
                accountId,
                firstName,
                lastName,
                zipCode,
                streetAddress,
                phoneNumber,
                faxNumber,
                userType
            ));

            if (user is null)
            {
                tx.Rollback();
                return new Err<AccountModel, Error>(new Error{
                    Message = "Failed to create the user profile.",
                    ErrorCode = "UserCreationFailed",
                    StatusCode = 500
                });
            }

            // アカウントの有効化
            var updatedAccount = account.ToDomain();
            updatedAccount.Activate();

            await accountRepository.UpdateAsync(updatedAccount);

            tx.Commit();

            return new Ok<AccountModel, Error>(new AccountModel(updatedAccount));
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }


    private async Task<AccountTable?> LockAccountAsync(Guid accountId)
    {
        return await conn.QueryFirstOrDefaultAsync<AccountTable>(
            "SELECT * FROM accounts WHERE id = @accountId FOR UPDATE",
            new { accountId }
        );
    }
}