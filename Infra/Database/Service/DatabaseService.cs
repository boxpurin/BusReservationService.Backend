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
            typeof(Domain.ValueObject.AccountStatus),
            new LowercaseEnumTypeHandler<Domain.ValueObject.AccountStatus>());

        SqlMapper.AddTypeHandler(
            typeof(Domain.ValueObject.AccountRole),
            new LowercaseEnumTypeHandler<Domain.ValueObject.AccountRole>());

        SqlMapper.AddTypeHandler(
            typeof(Domain.ValueObject.UserType),
            new LowercaseEnumTypeHandler<Domain.ValueObject.UserType>());

        SqlMapper.AddTypeHandler(
            typeof(Domain.ValueObject.ReservationStatus),
            new LowercaseEnumTypeHandler<Domain.ValueObject.ReservationStatus>());
    }

    public IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}
