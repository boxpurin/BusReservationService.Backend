using Dapper;
using System.Data;

namespace BusReservationService.Database.Interface;

public abstract class IConnection
{
    /// <summary>
    /// 接続が閉じている場合に接続を開く
    /// </summary>
    protected virtual void EnsureConnectionOpen(IDbConnection connection)
    {
        if (connection.State != ConnectionState.Open)
        {
            // IDbConnection には OpenAsync が拡張メソッドとして用意されていることが多いです
            // （Npgsql, Microsoft.Data.SqlClient など）
            if (connection is System.Data.Common.DbConnection dbConnection)
            {
                dbConnection.Open();
            }
            else
            {
                connection.Open();
            }
        }
    }
}