using System.Data;
using BusReservationService.Database.Repository;
using BusReservationService.Database.Service;
using BusReservationService.Database.Table;
using BusReservationService.Database.Test.Utils;
using BusReservationService.Domain.Entity;
using BusReservationService.Domain.Repository;
using BusReservationService.Domain.ValueObject.Prefecture;
using Dapper;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Respawn;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace BusReservationService.Database.Test.Repositories;

public class ParkingLotRepositoryTest : IClassFixture<DatabaseFixture>, IAsyncLifetime
{
    private readonly DatabaseFixture _fixture;
    private DatabaseService? dbService = null;
    private Respawner? _respawner = null;

    public ParkingLotRepositoryTest(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync ()
    {        
        var inMemorySettings = new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] = _fixture.ConnectionString
        };

        IConfiguration _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
            
        dbService = new DatabaseService(_configuration);
        dbService.ConfigureMappings();
        
        using var conn = dbService.CreateConnection();
        conn.Open();

        // 初回のみRespawnerを初期化
        if (_respawner == null)
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
    
    public async Task<IDbConnection> CreateConnectionAsync(string[]? seedSqlPaths = null)
    {
        var conn = dbService!.CreateConnection();

        if (seedSqlPaths is not null)
        {
            foreach(var path in seedSqlPaths)
            {
                var seedSqlPath = Path.GetFullPath(path);
                var seedSql = await File.ReadAllTextAsync(seedSqlPath);
                await conn.ExecuteAsync(seedSql);            
            }
        }

        return conn;
    }

    [Fact]
    public async Task AddParkingLot_Test()
    {
        IParkingLotRepository repository = new ParkingLotRepository(
            await CreateConnectionAsync()
        );

        var dto = new AddParkingLotDto(
            "AddTest駐車場", "1234567", Prefecture.Gifu, "東京都千代田区1-1-1", 
            "06-1234-1232", "06-6123-3213", 20, 10);

        var entity = await repository.AddAsync(dto);

        entity.Should().NotBeNull();
        entity.Should().BeEquivalentTo(dto);

        var insertedTable = await SelectParkingLotTableAsync(entity);
        insertedTable.ToDomain().Should().BeEquivalentTo(entity);
    }

    [Fact]
    public async Task UpdateParkingLot_Test()
    {
        using var conn = await CreateConnectionAsync();
        conn.Open();

        var seed = ParkingLotEntity.CreateNewParkingLotEntity(
            "TestInit駐車場", "1234567", Prefecture.Hokkaido,  "東京都千代田区1-1-1",
            "090-0000-0000", "06-0000-0000", 20, 10, true, true
        );

        var tables = await InsertParkingLotAsync(seed);
        var table = tables.First();

        var expect = ParkingLotEntity.FromSnapshot(
            table.Id,
            "Updated駐車場", 
            table.ZipCode, 
            PrefectureExtensions.FromIndex(table.PrefectureId), 
            table.StreetAddress, 
            table.PhoneNumber, 
            table.FaxNumber, 
            table.MaxCapacity, 
            table.ReservationCapacity, 
            table.IsAllowsBoardingAlighting, 
            table.IsActive
        );

        IParkingLotRepository repository = new ParkingLotRepository(
            conn
        );

        var updatedEntity = await repository.UpdateAsync(expect);

        updatedEntity.Should().NotBeNull();
        updatedEntity.Should().BeEquivalentTo(expect);

        var updatedTable = await SelectParkingLotTableAsync(expect);

        updatedTable.Id.Should().Be(updatedEntity.Id);
        updatedTable.DisplayName.Should().Be(updatedEntity.ParkingLotName);
    }

    [Fact]
    public async Task FindByPrefecture_Test()
    {
        IParkingLotRepository repository = new ParkingLotRepository(
            await CreateConnectionAsync(["Sql/Seeds/001_seeds.sql"])
        );

        var lots = await repository.FindByPrefactureIdAsync(Prefecture.Hokkaido);
        lots.Length.Should().Be(2);
        
        lots = await repository.FindByPrefactureIdAsync(Prefecture.Okinawa);
        lots.Length.Should().Be(0);
    }
    
    private async Task<ParkingLotTable[]> InsertParkingLotAsync(object? param = null)
    {
        using var conn = dbService!.CreateConnection();
        conn.Open();
        var tx = conn.BeginTransaction();
        var tables = await conn.QueryAsync<ParkingLotTable>("""
            INSERT INTO parking_lots(
                display_name, prefecture_id, zip_code, street_address, phone_number, fax_number,
                max_capacity, reservation_capacity, is_allows_boarding_alighting, is_active
            )
            VALUES (
                @ParkingLotName, @Prefecture, @ZipCode, @StreetAddress, @PhoneNumber, @FaxNumber,
                @MaxCapacity, @ReservationCapacity, @IsAllowsBoardingAlighting, @IsActive
            )
            RETURNING * ;
        """, param, tx);

        tx.Commit();

        return tables.ToArray();
    }

    private async Task<ParkingLotTable> SelectParkingLotTableAsync(ParkingLotEntity expect)
    {
        using var conn = dbService!.CreateConnection();
        conn.Open();
        var tx = conn.BeginTransaction();

        string sql = """
            SELECT * FROM parking_lots WHERE id = @Id;
        """;
        var table = await conn.QueryFirstAsync<ParkingLotTable>(sql, expect, tx);
        tx.Commit();

        return table;
    }
    
    public Task DisposeAsync() => Task.CompletedTask;
}