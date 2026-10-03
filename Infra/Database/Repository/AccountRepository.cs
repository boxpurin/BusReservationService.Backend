using BusReservationService.Database.Table;
using BusReservationService.Domain.Entity;
using BusReservationService.Domain.Repository;
using BusReservationService.Share;
using Dapper;
using System.Data;

namespace BusReservationService.Database.Repository;

public class AccountRepository : IAccountRepository
{
    private readonly IDbConnection _connection;

    public AccountRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<AccountEntity> AddAsync(AccountEntity account)
    {
        using var tx = _connection.BeginTransaction();

        const string sql = """
                INSERT INTO public.accounts (id, email, password_hash, password_salt, account_status, account_role)
                VALUES (@Id, @Email, @PasswordHash, @PasswordSalt @AccountStatus, @AccountRole)
                RETURNING * AS account;
            """;

        var ret = await _connection.QueryFirstAsync<AccountTable>(sql, account, tx);

        tx.Commit();

        return ret.ToDomain();
    }


    public async Task<AccountEntity> UpdateAsync(AccountEntity account)
    {
        using var tx = _connection.BeginTransaction();

        const string sql = """
                UPDATE public.accounts
                SET 
                email = @Email , 
                password_hash = @PasswordHash,
                password_salt = @PasswordSalt, 
                account_status = @AccountStatus ,
                account_role = @AccountRole , 
                lastLoginAt = @LastLoginAt 
                WHERE id = @Id
                RETURNING * AS account;
            """;

        var ret = await _connection.QueryFirstAsync<AccountTable>(sql, account, tx);

        tx.Commit();

        return ret.ToDomain();
    }


    public async Task<AccountEntity[]> GetAllAsync()
    {
        var sql = """
            SELECT * from public.accounts;
            """;

        var ret = await _connection.QueryAsync<AccountTable>(sql);

        return ret.Select(t => t.ToDomain()).ToArray();
    }

    public async Task<AccountEntity?> FindByAccountId(Guid accountId)
    {
        var sql = """
            SELECT * from public.accounts WHERE id = @Id;
            """;

        var ret = await _connection.QueryFirstAsync<AccountTable>(sql, new { Id = accountId});

        return ret?.ToDomain();
    }
    public async Task<AccountEntity?> FindByEmail(string email)
    {
        var sql = """
            SELECT * from public.accounts WHERE email = @Email;
            """;

        var ret = await _connection.QueryFirstAsync<AccountTable>(sql, new { Email = email });

        return ret?.ToDomain();
    }

    public async Task<bool> IsExistByAccountId(Guid accountId)
    {
        var sql = """
            SELECT * from public.accounts WHERE id = @Id;
            """;

        var ret = await _connection.QueryFirstAsync<AccountTable>(sql, new { Id = accountId });

        return ret is not null;

    }

    public async Task<bool> IsExistByEmail(string email)
    {
        var sql = """
            SELECT * from public.accounts WHERE email = @Email;
            """;

        var ret = await _connection.QueryFirstAsync<AccountTable>(sql, new { Email = email });

        return ret is not null;
    }

    public async Task<bool> DeleteListAsync(AccountEntity[] account)
    {
        throw new NotImplementedException();
    }
    public async Task<bool> DeleteAsync(AccountEntity account)
    {
        using var tx = _connection.BeginTransaction();
        var sql = """
            DELETE accounts WHERE Id = @Id;
            """;

        var ret = await _connection.ExecuteAsync(sql, account, tx);

        tx.Commit();

        return ret == 1;
    }
}
