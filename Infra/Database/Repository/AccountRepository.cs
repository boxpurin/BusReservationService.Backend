using BusReservationService.Database.Interface;
using BusReservationService.Database.Table;
using BusReservationService.Domain.Entity;
using BusReservationService.Domain.Repository;
using BusReservationService.Infra.Database;
using BusReservationService.Share;
using Dapper;
using System.Data;

namespace BusReservationService.Database.Repository;

public class AccountRepository(IDbContext context) : IAccountRepository
{
    public async Task<AccountEntity> AddAsync(AccountEntity account)
    {
        context.EnsureConnectionOpen();

        const string sql = """
                INSERT INTO public.accounts (id, email, password_hash, password_salt, account_status, account_role)
                VALUES (@Id, @Email, @PasswordHash, @PasswordSalt, @AccountStatus, @AccountRole)
                RETURNING *;
            """;

        var ret = await context.Connection.QueryFirstAsync<AccountTable>(sql, account, context.Transaction);

        return ret.ToDomain();
    }


    public async Task<AccountEntity> UpdateAsync(AccountEntity account)
    {
        context.EnsureConnectionOpen();

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

        var ret = await context.Connection.QueryFirstAsync<AccountTable>(sql, account, context.Transaction);

        return ret.ToDomain();
    }


    public async Task<AccountEntity[]> GetAllAsync()
    {
        context.EnsureConnectionOpen();
        var sql = """
            SELECT * from public.accounts;
            """;

        var ret = await context.Connection.QueryAsync<AccountTable>(sql, context.Transaction);

        return ret.Select(t => t.ToDomain()).ToArray();
    }

    public async Task<AccountEntity?> FindByAccountIdAsync(Guid accountId)
    {
        context.EnsureConnectionOpen();
        var sql = """
            SELECT * from public.accounts WHERE id = @Id;
            """;

        var ret = await context.Connection.QueryFirstOrDefaultAsync<AccountTable>(sql, new { Id = accountId}, context.Transaction);

        return ret?.ToDomain();
    }
    public async Task<AccountEntity?> FindByEmailAsync(string email)
    {
        context.EnsureConnectionOpen();
        var sql = """
            SELECT * from public.accounts WHERE email = @Email;
            """;

        var ret = await context.Connection.QueryFirstOrDefaultAsync<AccountTable>(sql, new { Email = email }, context.Transaction);

        return ret?.ToDomain();
    }

    public async Task<bool> IsExistByAccountIdAsync(Guid accountId)
    {
        context.EnsureConnectionOpen();
        var sql = """
            SELECT * from public.accounts WHERE id = @Id;
            """;

        var ret = await context.Connection.QueryFirstOrDefaultAsync<AccountTable>(sql, new { Id = accountId }, context.Transaction);

        return ret is not null;

    }

    public async Task<bool> IsExistByEmailAsync(string email)
    {
        context.EnsureConnectionOpen();
        var sql = """
            SELECT * from public.accounts WHERE email = @Email;
            """;

        var ret = await context.Connection.QueryFirstOrDefaultAsync<AccountTable>(sql, new { Email = email }, context.Transaction);

        return ret is not null;
    }
    
    public async Task<bool> DeleteAsync(AccountEntity account)
    {
        context.EnsureConnectionOpen();
        var sql = """
            DELETE FROM accounts WHERE id = @Id;
            """;
            
        var ret = await context.Connection.ExecuteAsync(sql, account, context.Transaction);

        return ret == 1;
    }
}
