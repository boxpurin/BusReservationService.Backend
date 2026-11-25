using Dapper;
using System.Data;

namespace BusReservationService.Database.Interface;

public abstract class IDbContext
{
    /// <summary>
    /// 接続が閉じている場合に接続を開く
    /// </summary>
    public virtual void EnsureConnectionOpen()
    {
        if (Connection.State != ConnectionState.Open)
        {
            // IDbConnection には OpenAsync が拡張メソッドとして用意されていることが多いです
            // （Npgsql, Microsoft.Data.SqlClient など）
            if (Connection is System.Data.Common.DbConnection dbConnection)
            {
                dbConnection.Open();
            }
            else
            {
                Connection.Open();
            }
        }
    }

    // 現在のワークに関連する接続情報を提供するインターフェース
    public IDbConnection Connection { get; protected set; } = default!;
    // 現在のトランザクション情報を提供するインターフェース
    public IDbTransaction? Transaction { get; protected set; }
}