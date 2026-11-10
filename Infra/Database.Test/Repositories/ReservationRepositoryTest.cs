using System.Data;
using BusReservationService.Database.Repository;
using BusReservationService.Database.Service;
using BusReservationService.Database.Table;
using BusReservationService.Database.Test.Utils;
using BusReservationService.Domain.Entity;
using BusReservationService.Domain.Repository;
using BusReservationService.Domain.ValueObject.Prefecture;
using BusReservationService.Domain.ValueObject.ReservationStatus;
using BusReservationService.Services.Interface;
using Shouldly;
using Dapper;
using Microsoft.Extensions.Configuration;
using Respawn;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace BusReservationService.Database.Test.Repositories;

public class ReservationRepositoryTest : IClassFixture<DatabaseFixture>, IAsyncLifetime
{
    private readonly DatabaseFixture _fixture;
    private IDatabaseService? dbService = null;
    private Respawner? _respawner = null;

    private readonly AccountEntity _existAccount = AccountEntity.CreateUserRoleAccount
    (
        "test@example.com", 
        [0x1, 0x2, 0x3, 0x4] , 
        [0x0, 0x1, 0x2, 0x3] 
    );

    private readonly AccountEntity _notExistAccount = AccountEntity.CreateUserRoleAccount
    (
        "NotExist@example.com", 
        [0x1, 0x2, 0x3, 0x4] , 
        [0x0, 0x1, 0x2, 0x3] 
    );


    private readonly ParkingLotEntity _existParkingLot = ParkingLotEntity.CreateNewParkingLotEntity
    (
        "Test駐車場", "1234567", 
        Prefecture.Tokyo, "東京都千代田区1-1-1",
        "06-1234-1234", null, 20, 10, false ,true
    );

    
    private readonly ReservationEntity[] ArrangeReservations;

    public ReservationRepositoryTest(DatabaseFixture fixture)
    {
        _fixture = fixture;
        
        
        var inComing = DateTime.Now;
        inComing = DateTime.UtcNow.AddTicks(-(inComing.Ticks % TimeSpan.TicksPerMinute));
        var OutGoing = inComing.AddHours(12);

        ArrangeReservations = new[] {
        ReservationEntity.CreateNewReservation
        (
            _existAccount.Id,
            _existParkingLot.Id,
            inComing,
            OutGoing,
            "Testバス1"
        ), 
        ReservationEntity.CreateNewReservation
        (
            _existAccount.Id,
            _existParkingLot.Id,
            inComing,
            OutGoing,
            "Testバス2"
        )};
    }

    public async ValueTask InitializeAsync()
    {
        var inMemorySettings = new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] = _fixture.ConnectionString
        };

        IConfiguration _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
            
        dbService = new NpgsqlDatabaseService(_configuration);
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
                    TablesToIgnore = new Respawn.Graph.Table[] { 
                        "prefectures",
                    }
            });
        }

        // 1. 全テーブルのデータを空にする
        await _respawner.ResetAsync((System.Data.Common.DbConnection)conn);

        // 2. データ追加
        var tx = conn.BeginTransaction();
        var _ = await conn.ExecuteAsync(
        """
            INSERT INTO public.accounts 
            (id, email, password_hash, password_salt, account_status, account_role)
            VALUES 
            (@Id, @Email, @PasswordHash, @PasswordSalt, @AccountStatus, @AccountRole);
        """, 
        _existAccount);

        _ = await conn.ExecuteAsync(
        """
            INSERT INTO parking_lots(
                id, display_name, prefecture_id, zip_code, street_address, phone_number, fax_number,
                max_capacity, reservation_capacity, is_allows_boarding_alighting, is_active
            )
            VALUES (
                @Id, @ParkingLotName, @Prefecture, @ZipCode, @StreetAddress, @PhoneNumber, @FaxNumber,
                @MaxCapacity, @ReservationCapacity, @IsAllowsBoardingAlighting, @IsActive
            );
        """, 
        _existParkingLot);
        tx.Commit();

        conn.Close();
    }

    [Fact]
    public async Task AddNewReservation_Test()
    {
        using var conn = await CreateConnectionAsync();
        IReservationRepository reservationRepository = new ReservationRepository(
            conn
        );
        var expected = ArrangeReservations.First();
        
        var added = await reservationRepository.AddAsync(expected);

        var inserted = await conn.QuerySingleOrDefaultAsync<ReservationTable>("""
            SELECT * FROM reservations
            WHERE id = @Id
        """, added);

        inserted!.ToDomain().ShouldBeEquivalentTo(added);

        expected = ReservationEntity.CreateNewReservation
        (
            _notExistAccount.Id,
            _existParkingLot.Id,
            expected.IncomingDateTime,
            expected.OutgoingDateTime,
            "Testバス"
        );
        
        Func<Task> act = async () => await reservationRepository.AddAsync(expected);
        await act.ShouldThrowAsync<Npgsql.PostgresException>();

    }

    [Fact]
    public async Task UpdateReservation_Test()
    {
        using var conn = await CreateConnectionAsync();
        IReservationRepository reservationRepository = new ReservationRepository(
            conn
        );

        var inComing = DateTime.Now;
        inComing = DateTime.UtcNow.AddTicks(-(inComing.Ticks % TimeSpan.TicksPerMinute));
        var OutGoing = inComing.AddHours(12);

        var arrange = ArrangeReservations.First();

        var table = await conn.QueryFirstAsync<ReservationTable>(
            """
            INSERT INTO reservations ( 
            id, account_id, parking_lot_id,
            incoming_datetime, outgoing_datetime, 
            reservation_status, bus_campany_name, canceled_at
            ) VALUES 
            (@Id, @AccountId, @ParkingLotId, 
            @IncomingDateTime, @OutGoingDateTime, 
            @ReservationStatus, @BusCampanyName, 
            @CanceledAt) 
            RETURNING * ;
            """
        , arrange);

        var expect = ReservationEntity.FromSnapshot(
            table.Id,
            table.AccountId,
            table.ParkingLotId,
            table.IncomingDateTime,
            table.OutgoingDateTime.AddHours(-6),
            ReservationStatus.Canceled,
            table.BusCampanyName,
            DateTime.Now
        );

        var updated = await reservationRepository.UpdateAsync(expect);

        var inserted = await conn.QuerySingleOrDefaultAsync<ReservationTable>("""
            SELECT * FROM reservations
            WHERE id = @Id
        """, updated);

        inserted!.ToDomain().ShouldBeEquivalentTo(updated);
        
    }

    [Fact]
    public async Task FindByReservationIdAsyncTest()
    {        
        using var conn = await CreateConnectionAsync();
        IReservationRepository reservationRepository = new ReservationRepository(
            conn
        );

        var inComing = DateTime.Now;
        inComing = DateTime.UtcNow.AddTicks(-(inComing.Ticks % TimeSpan.TicksPerMinute));
        var OutGoing = inComing.AddHours(12);

        var arrange = ArrangeReservations.First();

        var table = await conn.QueryFirstAsync<ReservationTable>(
            """
            INSERT INTO reservations ( 
            id, account_id, parking_lot_id,
            incoming_datetime, outgoing_datetime, 
            reservation_status, bus_campany_name, canceled_at
            ) VALUES 
            (@Id, @AccountId, @ParkingLotId, 
            @IncomingDateTime, @OutGoingDateTime, 
            @ReservationStatus, @BusCampanyName, 
            @CanceledAt) 
            RETURNING * ;
            """
        , arrange);

        var exist = await reservationRepository.FindByReservationIdAsync(arrange.Id);
        exist.ShouldNotBeNull();
        exist.Id.ShouldBe(arrange.Id);

        // 適当な存在しないIDを流す
        var notExist = await reservationRepository.FindByReservationIdAsync(_notExistAccount.Id);
        notExist.ShouldBeNull();
    }

    [Fact]
    public async Task FindByAccountIdAsyncTest()
    {        
        using var conn = await CreateConnectionAsync();
        IReservationRepository reservationRepository = new ReservationRepository(
            conn
        );

        var inComing = DateTime.Now;
        inComing = DateTime.UtcNow.AddTicks(-(inComing.Ticks % TimeSpan.TicksPerMinute));
        var OutGoing = inComing.AddHours(12);

        var tables = await conn.ExecuteAsync(
            """
            INSERT INTO reservations ( 
            id, account_id, parking_lot_id,
            incoming_datetime, outgoing_datetime, 
            reservation_status, bus_campany_name, canceled_at
            ) VALUES 
            (@Id, @AccountId, @ParkingLotId, 
            @IncomingDateTime, @OutGoingDateTime, 
            @ReservationStatus, @BusCampanyName, 
            @CanceledAt) ;
            """
        , ArrangeReservations);

        var exists = await reservationRepository.FindByAccountIdAsync(_existAccount.Id);
        exists.ShouldNotBeNull();
        exists.Length.ShouldBe(ArrangeReservations.Length);

        // 適当な存在しないIDを流す
        var notExist = await reservationRepository.FindByAccountIdAsync(_notExistAccount.Id);
        notExist.ShouldBeEmpty();
    }

    [Fact]
    public async Task FindByParkingLotIdAsyncTest()
    {        
        using var conn = await CreateConnectionAsync();
        IReservationRepository reservationRepository = new ReservationRepository(
            conn
        );

        var tables = await conn.ExecuteAsync(
            """
            INSERT INTO reservations ( 
            id, account_id, parking_lot_id,
            incoming_datetime, outgoing_datetime, 
            reservation_status, bus_campany_name, canceled_at
            ) VALUES 
            (@Id, @AccountId, @ParkingLotId, 
            @IncomingDateTime, @OutGoingDateTime, 
            @ReservationStatus, @BusCampanyName, 
            @CanceledAt) ;
            """
        , ArrangeReservations);

        var exists = await reservationRepository.FindByParkingLotIdAsync(_existParkingLot.Id);
        exists.ShouldNotBeNull();
        exists.Length.ShouldBe(ArrangeReservations.Length);

        // 適当な存在しないIDを流す
        var notExist = await reservationRepository.FindByParkingLotIdAsync(_notExistAccount.Id);
        notExist.ShouldBeEmpty();
    }

    [Fact]
    public async Task DeleteReservation_Test()
    {       
        using var conn = await CreateConnectionAsync();
        IReservationRepository reservationRepository = new ReservationRepository(
            conn
        );
        
        var tables = await conn.ExecuteAsync(
            """
            INSERT INTO reservations ( 
            id, account_id, parking_lot_id,
            incoming_datetime, outgoing_datetime, 
            reservation_status, bus_campany_name, canceled_at
            ) VALUES 
            (@Id, @AccountId, @ParkingLotId, 
            @IncomingDateTime, @OutGoingDateTime, 
            @ReservationStatus, @BusCampanyName, 
            @CanceledAt) ;
            """
        , ArrangeReservations);

        var target = ArrangeReservations.First();

        var result = await reservationRepository.DeleteByReservationIdAsync(target.Id);
        result.ShouldBeTrue();
        
        result = await reservationRepository.DeleteByReservationIdAsync(target.Id);
        result.ShouldBeFalse();
        
    }

    public async Task<IDbConnection> CreateConnectionAsync(string[]? seedSqlPaths = null)
    {
        var conn = dbService!.CreateConnection();
        dbService!.ConfigureMappings();

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

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}