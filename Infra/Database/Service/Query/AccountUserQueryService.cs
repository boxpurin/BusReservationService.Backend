using System.Data;
using BusReservationService.Database.Interface;
using BusReservationService.Models;
using BusReservationService.Services.Interface;
using Dapper;

namespace BusReservationService.Infra.Database.Service.Query;

/// <summary>
/// アカウント情報とユーザー情報をまとめたデータを取得するサービス
/// </summary>
/// <param name="DbConnection">データベース接続</param>
public class AccountUserQueryService(IDbConnection DbConnection)
: IConnection, IAccountUserQueryService
{
    public async Task<AccountUserModel?> GetModelByAccountIdAsync(Guid accountId)
    {
        EnsureConnectionOpen(DbConnection);

        string sql = """
        SELECT a.id AS "AccountId", a.email, a.password_hash, a.password_salt, u.first_name, u.last_name, u.phone_number, a.account_role, a.account_status, u.user_type
        FROM public.accounts AS a
        LEFT JOIN public.users AS u
        ON a.id = u.account_id
        WHERE a.id = @AccountId
        """;

        var model = await DbConnection.QuerySingleOrDefaultAsync<AccountUserModel>(
            sql, 
            new { AccountId = accountId }
        );

        return model;
    }

    public async Task<AccountUserModel?> GetModelByEmailAsync(string email)
    {
        EnsureConnectionOpen(DbConnection);

        string sql = """
        SELECT a.id AS "AccountId" , a.email, a.password_hash, a.password_salt, u.first_name, u.last_name, u.phone_number, a.account_role, a.account_status, u.user_type
        FROM public.accounts AS a
        LEFT JOIN public.users AS u
        ON a.id = u.account_id
        WHERE a.email = @Email
        """;

        var model = await DbConnection.QuerySingleOrDefaultAsync<AccountUserModel>(
            sql, 
            new { Email = email }
        );

        return model;
    }
}