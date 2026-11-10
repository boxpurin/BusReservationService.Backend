using System.Data;
using BusReservationService.Database.Enums;
using Microsoft.Extensions.Configuration;
using Dapper;
using Npgsql;
using BusReservationService.Services.Interface;

namespace BusReservationService.Database.Service;

/// <summary>
/// Npgsqlを使用したデータベースサービスの実装
/// </summary>
public class NpgsqlDatabaseService : IDatabaseService
{
    string _connectionString;

    public NpgsqlDatabaseService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
            "DB connection string is not configured.");

        ConfigureMappings();
    }

    // Dapperのマッピング設定
    public void ConfigureMappings()
    {
        SqlMapper.Settings.PreferTypeHandlersForEnums = true;
        DefaultTypeMap.MatchNamesWithUnderscores = true;
        
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
        
        SqlMapper.AddTypeHandler(
            typeof(Domain.ValueObject.AccountStatus.AccountStatus),
            new LowercaseEnumTypeHandler<Domain.ValueObject.AccountStatus.AccountStatus>());

        SqlMapper.AddTypeHandler(
            typeof(Domain.ValueObject.AccountRole.AccountRole),
            new LowercaseEnumTypeHandler<Domain.ValueObject.AccountRole.AccountRole>());

        SqlMapper.AddTypeHandler(
            typeof(Domain.ValueObject.UserType.UserType),
            new LowercaseEnumTypeHandler<Domain.ValueObject.UserType.UserType>());

        SqlMapper.AddTypeHandler(
            typeof(Domain.ValueObject.ReservationStatus.ReservationStatus),
            new LowercaseEnumTypeHandler<Domain.ValueObject.ReservationStatus.ReservationStatus>());
    }

    public IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}
