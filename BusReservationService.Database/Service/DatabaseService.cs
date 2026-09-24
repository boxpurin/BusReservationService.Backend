using BusReservationService.Database.Enum;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

namespace BusReservationService.Database.Service;

public class DatabaseService
{
    string _connectionString;

    public DatabaseService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
            "DB connection string is not configured.");
    }

    // Dapperのマッピング設定
    public void ConfigureMappings()
    {
        SqlMapper.AddTypeHandler(
            typeof(AccountStatus),
            new LowercaseEnumTypeHandler<AccountStatus>());

        SqlMapper.AddTypeHandler(
            typeof(AccountRole),
            new LowercaseEnumTypeHandler<AccountRole>());
    }

    public IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}
