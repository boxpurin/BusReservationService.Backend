using BusReservationService.Database.Service;
using DotNet.Testcontainers.Configurations;
using Testcontainers.PostgreSql;

namespace BusReservationService.Database.Test.Utils;

public class DatabaseFixture : IAsyncLifetime
{
    private DatabaseService? dbService = null;

    public PostgreSqlContainer DbContainer { get; } = new PostgreSqlBuilder("postgres:16-alpine")
    .WithDatabase("test_db")
    .WithUsername("postgres")
    .WithPassword("password")
    .WithBindMount(
        Path.GetFullPath("Migrations/20260911084734_init.sql"), 
        "/docker-entrypoint-initdb.d/01_init.sql", 
        AccessMode.ReadOnly)
    .WithBindMount(
        Path.GetFullPath("Sql/Seeds/001_accounts.sql"), 
        "/docker-entrypoint-initdb.d/02_accounts.sql", 
        AccessMode.ReadOnly)
    .Build();

    public string ConnectionString => DbContainer.GetConnectionString();

    public async Task InitializeAsync()
    {
        await DbContainer.StartAsync();
    }

    public Task DisposeAsync() => DbContainer.DisposeAsync().AsTask();
}