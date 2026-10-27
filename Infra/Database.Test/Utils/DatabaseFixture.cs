using BusReservationService.Database.Service;
using DotNet.Testcontainers.Configurations;
using Testcontainers.PostgreSql;

namespace BusReservationService.Database.Test.Utils;

public class DatabaseFixture : IAsyncLifetime
{
    public PostgreSqlContainer DbContainer { get; } = new PostgreSqlBuilder("postgres:16-alpine")
    .WithDatabase("test_db")
    .WithUsername("postgres")
    .WithPassword("password")
    .WithBindMount(
        Path.GetFullPath("Migrations/20260911084734_init.sql"), 
        "/docker-entrypoint-initdb.d/001_init.sql", 
        AccessMode.ReadOnly)
    .WithBindMount(
        Path.GetFullPath("Migrations/20261017011239_prefecture.sql"), 
        "/docker-entrypoint-initdb.d/002_prefecture.sql", 
        AccessMode.ReadOnly)
    .Build();

    public string ConnectionString => DbContainer.GetConnectionString();

    public async ValueTask InitializeAsync()
    {
        await DbContainer.StartAsync();
    }

    public ValueTask DisposeAsync() => DbContainer.DisposeAsync();
}