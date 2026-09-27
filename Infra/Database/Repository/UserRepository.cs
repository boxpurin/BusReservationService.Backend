using BusReservationService.Database.Table;
using BusReservationService.Domain.Entity;
using BusReservationService.Domain.Repository;
using BusReservationService.Share;
using Dapper;
using System.Data;


namespace BusReservationService.Database.Repository;

public class UserRepository(IDbConnection Connection) : IUserRepository
{
    public async Task<UserEntity> AddAsymc(UserEntity entity)
    {
        using var tx = Connection.BeginTransaction();

        const string sql = """
                INSERT INTO users (account_id, first_name, last_name, phone_number, user_type)
                VALUE(@AccountId, @Firstname, @LastName, @PhoneNumber, @UserType)
                WHERE account_id = @AccountId;
            """;

        var ret = await Connection.ExecuteAsync(sql, entity);

        tx.Commit();

        return entity;
    }

    public async Task<UserEntity[]> AddListAsync(UserEntity[] entity)
    {
        using var tx = Connection.BeginTransaction();

        const string sql = @"
                INSERT INTO users (account_id, first_name, last_name, phone_number, user_type)
                SELECT t.account_id, t.first_name, t.last_name t.phone_number, t.user_type
                FROM unnest(@AccountIds, @FirstNames, @LastNames, @PhoneNumbers, @UserTypes) 
                    AS t(account_id, first_name, last_name, phone_number, user_type)
                WHERE EXISTS (
                    SELECT 1 FROM account a WHERE a.account_id = t.account_id
                );";

        var ret = await Connection.ExecuteAsync(sql, new
        {
            AccountIds = entity.Select(u => u.AccountId).ToArray(),
            FirstNames = entity.Select(u => u.FirstName).ToArray(),
            LastNames = entity.Select(u => u.LastName).ToArray(),
            PhoneNumbers = entity.Select(u => u.PhoneNumber).ToArray(),
            UserTypes = entity.Select(u => u.UserType).ToArray(),
        });

        tx.Commit();

        return entity;
    }


    public async Task<UserEntity> UpdateAsync(UserEntity entity)
    {
        using var tx = Connection.BeginTransaction();
        const string sql = """
                UPDATE users
                SET account_id = @AccountId ,
                first_name = @FirstName ,
                last_name = @LastName ,
                phone_number = @PhoneNumber ,
                user_type = @UserType 
                WHERE account_id = @AccountId            
            """;

        var ret = await Connection.QueryFirstAsync<UserTable>(sql, entity);
        tx.Commit();

        return ret.ToDomain();
    }

    public async Task<UserEntity[]> UpdateListAsync(UserEntity[] entity)
    {
        using var tx = Connection.BeginTransaction();
        const string sql = @"
                UPDATE users
                SET account_id = t.account_id ,
                first_name = t.first_name ,
                last_name = t.last_name ,
                phone_numbe = t.phone_numbe ,
                user_type = t.user_type
                FROM unnest(@AccountIds, @FirstNames, @LastNames, @PhoneNumbers, @UserTypes) 
                    AS t(account_id, first_name, last_name, phone_number, user_type)
                WHERE EXISTS (
                    SELECT 1 FROM account a WHERE a.account_id = t.account_id
                );";

        var ret = await Connection.ExecuteAsync(sql, new
        {
            AccountIds = entity.Select(u => u.AccountId).ToArray(),
            FirstNames = entity.Select(u => u.FirstName).ToArray(),
            LastNames = entity.Select(u => u.LastName).ToArray(),
            PhoneNumbers = entity.Select(u => u.PhoneNumber).ToArray(),
            UserTypes = entity.Select(u => u.UserType).ToArray(),
        });

        tx.Commit();
        return entity;
    }

    public async Task<UserEntity?> FindByAccountId(Guid accountId)
    {
        string sql = "SELECT * FROM users WHERE account_id = @AccountId";

        var ret = await Connection.QueryFirstAsync<UserTable>(sql, new { AccountId = accountId });
        
        return ret?.ToDomain();
    }
    public async Task<bool> ExistByAccountId(Guid accountId)
    {
        string sql = "SELECT * FROM users WHERE account_id = @AccountId";

        var ret = await Connection.QueryFirstAsync<UserTable>(sql, new { AccountId = accountId });

        return ret is not null;
    }


    public async Task<bool> DeleteById(Guid accountId)
    {
        using var tx = Connection.BeginTransaction();
        await Connection.ExecuteAsync("DELETE users WHERE account_id = @AccountId", new { AccountId = accountId });

        tx.Commit();

        return true;
    }

}