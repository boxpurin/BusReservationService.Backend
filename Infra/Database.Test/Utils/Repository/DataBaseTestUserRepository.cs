using BusReservationService.Database.Interface;
using BusReservationService.Database.Repository;
using BusReservationService.Domain.Entity;
using BusReservationService.Database.Table;
using Dapper;
using BusReservationService.Domain.Repository;
using BusReservationService.Database.Enums;

namespace BusReservationService.Infra.Database.Test.Utils.Repository;

/// <summary>
/// テストケースで使用するヘルパーリポジトリ
/// テストケースのための追加、更新、全件取得、削除を行うリポジトリ
/// </summary>
/// <param name="context">接続セッション</param>
internal class DataBaseTestUserRepository(IDbContext context)
{
    public async Task<UserEntity> AddAsync(
        Guid accountId,
        string firstName,
        string lastName,
        string zipCode = "1234567",
        string streetAddress="default street",
        string phoneNumber="000-0000-0000",
        string faxNumber="000-0000-0000",
        UserType userType = UserType.Personal)
    {
        var user = UserEntity.CreateUserEntity(
            accountId,
            firstName,
            lastName,
            zipCode,
            streetAddress,
            phoneNumber,
            faxNumber,
            userType.ToDomain()
        );
        context.EnsureConnectionOpen();

        const string sql = """
                INSERT INTO users 
                (account_id, first_name, last_name, zip_code, street_address, phone_number, fax_number, user_type)
                VALUES (@AccountId, @Firstname, @LastName, @ZipCode, @StreetAddress, @PhoneNumber, @FaxNumber, @UserType)
                RETURNING *;
            """;

        var userTable = await context.Connection.QueryFirstAsync<UserTable>(
            sql,
            new {
                AccountId = accountId,
                FirstName = firstName,
                LastName = lastName,
                ZipCode = zipCode,
                StreetAddress = streetAddress,
                PhoneNumber = phoneNumber,
                FaxNumber = faxNumber,
                UserType = userType
            },
            context.Transaction
        );

        return userTable.ToDomain();
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

    public async Task<UserEntity[]> GetAllAsync()
    {
        context.EnsureConnectionOpen();

        var sql = """
            SELECT * FROM users;
        """;

        var tables = await context.Connection.QueryAsync<UserTable>(sql, transaction: context.Transaction);

        if(tables.FirstOrDefault() is null)
        {
            return [];
        }

        return tables.Select(t => t.ToDomain()).ToArray();
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