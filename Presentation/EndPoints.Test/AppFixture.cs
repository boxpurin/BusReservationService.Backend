using FastEndpoints.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using BusReservationService;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;
using Microsoft.Extensions.Configuration;
using DotNet.Testcontainers.Configurations;
using Respawn;
using System.Data;
using BusReservationService.Services.Interface;

namespace BusReservationService.Endpoints.Test;

public class MyApp : AppFixture<ApiMarker>
{
    PostgreSqlContainer _dbContainer =  new PostgreSqlBuilder("postgres:16-alpine")
        .WithDatabase("TestDb")
        .WithUsername("postgres")
        .WithPassword("password123")
        .WithBindMount(
            Path.GetFullPath("Migrations/20260911084734_init.sql"),
            "/docker-entrypoint-initdb.d/001_init.sql", 
            AccessMode.ReadOnly)
        .WithBindMount(
            Path.GetFullPath("Migrations/20261017011239_prefecture.sql"),
            "/docker-entrypoint-initdb.d/002_prefecture.sql",
            AccessMode.ReadOnly)
        .Build();
    
    RedisContainer _redisContainer = new RedisBuilder("redis:8.10-alpine")
    .Build();

    private Respawner? _respawner;

    protected override async ValueTask PreSetupAsync()
    {
        await _dbContainer.StartAsync();
        await _redisContainer.StartAsync();
    }

    protected override ValueTask SetupAsync()
    {
        return default;
    }
    
    protected async override void ConfigureApp(IWebHostBuilder b)
    {
        // do host builder config here
        b.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:DefaultConnection", _dbContainer.GetConnectionString() },
                { "ConnectionStrings:Redis", _redisContainer.GetConnectionString() }
            });
        });
    }

    protected async override void ConfigureServices(IServiceCollection s)
    {
        // do test service registration here
    }

    public async Task ResetDatabaseAsync()
    {
        var key = Services.GetRequiredService<IDatabaseService>();
        var conn = key.CreateConnection();
        conn.Open();
        
        // 初回のみRespawnerを初期化
        if (_respawner is null)
        {
            _respawner = await Respawner.CreateAsync(
                (System.Data.Common.DbConnection)conn,
                new RespawnerOptions{
                    DbAdapter = DbAdapter.Postgres,
                    TablesToIgnore = new Respawn.Graph.Table[] { "prefectures" }
            });
        }

        // 1. 全テーブルのデータを空にする
        await _respawner.ResetAsync((System.Data.Common.DbConnection)conn);
    }

    protected override ValueTask TearDownAsync() => ValueTask.CompletedTask;
}