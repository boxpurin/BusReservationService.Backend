using BusReservationService.Database.Enums;
using BusReservationService.Database.Interface;
using BusReservationService.Database.Table;
using BusReservationService.Domain.Entity;
using BusReservationService.Domain.Repository;
using BusReservationService.Share;
using Dapper;
using System.Data;


namespace BusReservationService.Database.Repository;

public class UserRepository(IDbContext context) : IUserRepository
{
    public async Task<UserEntity> AddAsync(AddAsyncDto dto)
    {
        context.EnsureConnectionOpen();

        const string sql = """
                INSERT INTO users 
                (account_id, first_name, last_name, zip_code, street_address, phone_number, fax_number, user_type)
                VALUES (@AccountId, @Firstname, @LastName, @ZipCode, @StreetAddress, @PhoneNumber, @FaxNumber, @UserType)
                RETURNING *;
            """;

        var userTable = await context.Connection.QueryFirstAsync<UserTable>(
            sql,
            dto,
            context.Transaction
        );

        return UserEntity.
            CreateUserEntity(
            dto.AccountId,
            userTable.FirstName,
            userTable.LastName,
            userTable.ZipCode,
            userTable.StreetAddress,
            userTable.PhoneNumber,
            userTable.FaxNumber,
            userTable.UserType.ToDomain());
    }

    public async Task<UserEntity> UpdateAsync(UserEntity entity)
    {
        context.EnsureConnectionOpen();

        const string sql = """
                UPDATE users
                SET 
                account_id = @AccountId ,
                first_name = @FirstName ,
                last_name = @LastName ,
                zip_code = @ZipCode ,
                street_address = @StreetAddress ,
                phone_number = @PhoneNumber ,
                fax_number = @FaxNumber ,
                user_type = @UserType 
                WHERE account_id = @AccountId 
                RETURNING * ;
            """;

        var userTable = await context.Connection.QueryFirstAsync<UserTable>(
            sql,
            entity,
            context.Transaction
        );

        return userTable.ToDomain();
    }

    public async Task<UserEntity?> FindByAccountIdAsync(Guid accountId)
    {
        context.EnsureConnectionOpen();
        string sql = "SELECT * FROM users WHERE account_id = @AccountId";

        var userTable = await context.Connection.QueryFirstOrDefaultAsync<UserTable>(
            sql,
            new { AccountId = accountId },
            context.Transaction 
        );

        if ( userTable is null )
        {
            return null;
        }
        
        return UserEntity.
            CreateUserEntity(
            userTable.AccountId,
            userTable.FirstName,
            userTable.LastName,
            userTable.ZipCode,
            userTable.StreetAddress,
            userTable.PhoneNumber,
            userTable.FaxNumber,
            userTable.UserType.ToDomain());
    }
    public async Task<bool> ExistByAccountIdAsync(Guid accountId)
    {
        context.EnsureConnectionOpen();

        string sql = "SELECT * FROM users WHERE account_id = @AccountId";

        var ret = await context.Connection.QueryFirstOrDefaultAsync<UserTable>(
            sql,
            new { AccountId = accountId },
            context.Transaction
        );

        return ret is not null;
    }


    public async Task<bool> DeleteByAccountIdAsync(Guid accountId)
    {
        context.EnsureConnectionOpen();
        
        var ret = await context.Connection.ExecuteAsync(
            "DELETE FROM users WHERE account_id = @AccountId",
            new { AccountId = accountId },
            context.Transaction
        );

        return ret == 1;
    }
}