using System.Data;
using BusReservationService.Models;
using Dapper;

namespace BusReservationService.Services.Query;
/// <summary>
/// アカウント情報とユーザー情報をまとめたデータを取得するサービス
/// </summary>
/// <param name="DbConnection">データベース接続</param>
public class AccountUserQueryService(IDbConnection DbConnection) 
: IAccountUserQueryService
{    
    public async Task<AccountUserModel?> GetModelByAccountIdAsync(Guid accountId)
    {
        string sql = """
        SELECT a.id , a.email, a.password, u.first_name, u.last_name, u.phone_number, a.account_role, a.account_status, u.user_type
        FROM public.accounts AS a
        INNER JOIN public.users AS u
        ON a.id = u.account_id
        WHERE a.id = @AccountId
        """;

        var model = await DbConnection.QueryFirstAsync<AccountUserModel>(
            sql, 
            new { AccountId = accountId }
        );

        return model;
    }

    public async Task<AccountUserModel?> GetModelByEmailAsync(string email)
    {
        string sql = """
        SELECT a.id , a.email, a.password, u.first_name, u.last_name, u.phone_number, a.account_role, a.account_status, u.user_type
        FROM public.accounts AS a
        INNER JOIN public.users AS u
        ON a.id = u.account_id
        WHERE a.email = @Email
        """;

        var model = await DbConnection.QueryFirstAsync<AccountUserModel>(
            sql, 
            new { Email = email }
        );

        return model;
    }
}