using BusReservationService.Database.Interface;
using BusReservationService.Database.Repository;
using BusReservationService.Infra.Database;
using BusReservationService.Domain.Entity;
using BusReservationService.Database.Table;
using Dapper;


namespace BusReservationService.Infra.Database.Test.Utils.Repository;

/// <summary>
/// テストケースで使用するヘルパーリポジトリ
/// テストケースのための追加、更新、全件取得、削除を行うリポジトリ
/// </summary>
/// <param name="context">接続セッション</param>
internal class DataBaseTestAccountRepository(IDbContext context)
{
    // AccountEntityの追加メソッド
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

    // AccountEntityの更新メソッド
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

        var ret = await context.Connection.QueryAsync<AccountTable>(sql, transaction: context.Transaction);

        return ret.Select(t => t.ToDomain()).ToArray();
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