using System.Data;
using BusReservationService.Database.Enums;
using Microsoft.Extensions.Configuration;
using Dapper;
using Npgsql;

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

        SqlMapper.AddTypeHandler(
            typeof(UserType),
            new LowercaseEnumTypeHandler<UserType>());
        
        SqlMapper.AddTypeHandler(
            typeof(ReservationStatus),
            new LowercaseEnumTypeHandler<ReservationStatus>());
        
    }

    public IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}
