using BusReservationService.Database.Interface;
using BusReservationService.Database.Table;
using BusReservationService.Domain.Entity;
using BusReservationService.Domain.Repository;
using BusReservationService.Share;
using Dapper;
using System.Data;

namespace BusReservationService.Database.Repository;

public class AccountRepository(IDbConnection connection) : IConnection, IAccountRepository
{
    public async Task<AccountEntity> AddAsync(AccountEntity account)
    {
        EnsureConnectionOpen(connection);

        using var tx = connection.BeginTransaction();

        const string sql = """
                INSERT INTO public.accounts (id, email, password_hash, password_salt, account_status, account_role)
                VALUES (@Id, @Email, @PasswordHash, @PasswordSalt, @AccountStatus, @AccountRole)
                RETURNING *;
            """;

        var ret = await connection.QueryFirstAsync<AccountTable>(sql, account, tx);

        tx.Commit();

        return ret.ToDomain();
    }


    public async Task<AccountEntity> UpdateAsync(AccountEntity account)
    {
        EnsureConnectionOpen(connection);
        using var tx = connection.BeginTransaction();

        const string sql = """
                UPDATE public.accounts
                SET 
                email = @Email , 
                password_hash = @PasswordHash,
                password_salt = @PasswordSalt, 
                account_status = @AccountStatus ,
                account_role = @AccountRole
                WHERE id = @Id
                RETURNING *;
            """;

        var ret = await connection.QueryFirstAsync<AccountTable>(sql, account, tx);

        tx.Commit();

        return ret.ToDomain();
    }


    public async Task<AccountEntity[]> GetAllAsync()
    {
        EnsureConnectionOpen(connection);
        var sql = """
            SELECT * from public.accounts;
            """;

        var ret = await connection.QueryAsync<AccountTable>(sql);

        return ret.Select(t => t.ToDomain()).ToArray();
    }

    public async Task<AccountEntity?> FindByAccountId(Guid accountId)
    {
        EnsureConnectionOpen(connection);
        var sql = """
            SELECT * from public.accounts WHERE id = @Id;
            """;

        var ret = await connection.QueryFirstOrDefaultAsync<AccountTable>(sql, new { Id = accountId});

        return ret?.ToDomain();
    }
    public async Task<AccountEntity?> FindByEmail(string email)
    {
        EnsureConnectionOpen(connection);
        var sql = """
            SELECT * from public.accounts WHERE email = @Email;
            """;

        var ret = await connection.QueryFirstOrDefaultAsync<AccountTable>(sql, new { Email = email });

        return ret?.ToDomain();
    }

    public async Task<bool> IsExistByAccountId(Guid accountId)
    {
        EnsureConnectionOpen(connection);
        var sql = """
            SELECT * from public.accounts WHERE id = @Id;
            """;

        var ret = await connection.QueryFirstOrDefaultAsync<AccountTable>(sql, new { Id = accountId });

        return ret is not null;

    }

    public async Task<bool> IsExistByEmail(string email)
    {
        EnsureConnectionOpen(connection);
        var sql = """
            SELECT * from public.accounts WHERE email = @Email;
            """;

        var ret = await connection.QueryFirstOrDefaultAsync<AccountTable>(sql, new { Email = email });

        return ret is not null;
    }
    
    public async Task<bool> DeleteAsync(AccountEntity account)
    {
        EnsureConnectionOpen(connection);
        using var tx = connection.BeginTransaction();
        var sql = """
            DELETE FROM accounts WHERE Id = @Id;
            """;

        var ret = await connection.ExecuteAsync(sql, account, tx);

        tx.Commit();

        return ret == 1;
    }
}
