using BusReservationService.Database.Test.Utils;
using BusReservationService.Services.Interface;
using Respawn;
using Shouldly;
using Xunit.v3;
using Microsoft.Extensions.Configuration;
using Dapper;
using BusReservationService.Database.Service;
using BusReservationService.Application.Core.Services.Interface;
using BusReservationService.Infra.Database.Service;
using BusReservationService.Domain.Repository;
using BusReservationService.Database.Repository;
using BusReservationService.Infra.Database.Test.Utils.Repository;
using BusReservationService.Domain.Entity;
using BusReservationService.Database.Enums;
using BusReservationService.Domain.ValueObject.Unit;
using BusReservationService.Application.Core.Models;

namespace BusReservationService.Infra.Database.Test.Services;


public class ParkingLotAvailabilityListServiceTest : IClassFixture<DatabaseFixture>, IAsyncLifetime
{
    DatabaseFixture _fixture;
    private IDatabaseService? dbService = null;
    private Respawner? _respawner = null;
    
    public ParkingLotAvailabilityListServiceTest(DatabaseFixture fixture)
    {
        this._fixture = fixture;
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
                    TablesToIgnore = new Respawn.Graph.Table[] { "prefectures" }
            });
        }

        // 1. 全テーブルのデータを空にする
        await _respawner.ResetAsync((System.Data.Common.DbConnection)conn);
    }

    // DbSession を生成して返すファクトリメソッド
    public async Task<DbSession> CreateConnectionAsync(string[]? seedSqlPaths = null)
    {
        var conn = dbService!.CreateConnection();
        DbSession dbSession = new DbSession(conn);

        if (seedSqlPaths is not null)
        {
            foreach(var path in seedSqlPaths)
            {
                var seedSqlPath = Path.GetFullPath(path);
                var seedSql = await File.ReadAllTextAsync(seedSqlPath);
                await dbSession.Connection.ExecuteAsync(seedSql);
            }
        }

        return dbSession;
    }

    [Theory]
    [InlineData(15, 96)]
    [InlineData(30, 48)]
    [InlineData(60, 24)]
    public async Task Expected_ParkingLotAvailabilityListService_Slots_Count_Test(
        int slotIntervalMinutes, int expectedSlotCount)
    {
        // Arrange
        var (account, parkingLot, testDt) = await SeedAccountAndParkingLotAsync();
        var arrange = async () => {
            using var session = await CreateConnectionAsync();
            await session.BeginAsync();

            DataBaseTestReservationRepository reservationRepository =
            new DataBaseTestReservationRepository(session);

            var reservations = new[] {
                ReservationEntity.CreateNewReservation(
                    new ReservationNumberUnit("000000"),
                    account.Id,
                    parkingLot.Id,
                    testDt,
                    testDt.AddHours(12),
                    "BusCompany"
                )
            };

            foreach (var r in reservations)
            {
                await reservationRepository.AddAsync(r);
            }

            await session.CommitAsync();

            return (parkingLot, testDt);
        };

        await arrange.Invoke();

        // Act
        using var session = await CreateConnectionAsync();

        IParkingLotRepository repository = new ParkingLotRepository(session);
        IReservationRepository reservationRepository = new ReservationRepository(session);

        var options = new ParkingLotAvailabilityOptions
        {
            SlotInterval = TimeSpan.FromMinutes(slotIntervalMinutes)
        };

        IParkingLotAvailabilityListService service = new ParkingLotAvailabilityListService(
            reservationRepository,
            repository,
            options
        );

        var availability = (await service.GetDailyAvailabilityAsync(
            parkingLot.Id,
            DateOnly.FromDateTime(testDt)
        )).Unwrap();

        // Assert
        availability.ShouldNotBeNull();
        availability.AvailabilitySlots.Count.ShouldBe(expectedSlotCount);
        availability.SlotIntervalMinutes.ShouldBe(options.SlotInterval);

        var slotStartExpected = testDt;
        var slotEndExpected = testDt.AddMinutes(slotIntervalMinutes);

        foreach(var slot in availability.AvailabilitySlots)
        {
            slot.StartTime.ShouldBe(slotStartExpected);
            slot.EndTime.ShouldBe(slotEndExpected);
            slot.MaxReservationCapacity.ShouldBe(parkingLot.ReservationCapacity);
            slot.RemainingCapacity.ShouldBeLessThanOrEqualTo(slot.MaxReservationCapacity);

            slotStartExpected = slotEndExpected;
            slotEndExpected = slotStartExpected.AddMinutes(slotIntervalMinutes);
        }
    }
    
    [Theory]
    [InlineData(30)]
    [InlineData(60)]
    public async Task Expected_ReservedCount_Per_Slot_With_Overlapping_And_Boundary_Reservations_Test(
        int slotIntervalMinutes)
    {
        var (account, parkingLot, testDt) = await SeedAccountAndParkingLotAsync();
        var arrange = async () => {
            using var session = await CreateConnectionAsync();
            await session.BeginAsync();

            DataBaseTestReservationRepository reservationRepository =
            new DataBaseTestReservationRepository(session);

            var reservations = new[] {
                ReservationEntity.CreateNewReservation(
                    new ReservationNumberUnit("000000"),
                    account.Id,
                    parkingLot.Id,
                    testDt.AddHours(4),
                    testDt.AddHours(12),
                    "BusCompany"
                ),
                ReservationEntity.CreateNewReservation(
                    new ReservationNumberUnit("000001"),
                    account.Id,
                    parkingLot.Id,
                    testDt.AddHours(8),
                    testDt.AddHours(16),
                    "BusCompany"
                ),
                ReservationEntity.CreateNewReservation(
                    new ReservationNumberUnit("000002"),
                    account.Id,
                    parkingLot.Id,
                    testDt.AddHours(12),
                    testDt.AddHours(20),
                    "BusCompany"
                )
            };

            foreach (var r in reservations)
            {
                await reservationRepository.AddAsync(r);
            }

            await session.CommitAsync();

            return (parkingLot, testDt);
        };

        await arrange.Invoke();

        using var session = await CreateConnectionAsync();

        IParkingLotRepository repository = new ParkingLotRepository(session);
        IReservationRepository reservationRepository = new ReservationRepository(session);

        var options = new ParkingLotAvailabilityOptions
        {
            SlotInterval = TimeSpan.FromMinutes(slotIntervalMinutes)
        };

        IParkingLotAvailabilityListService service = new ParkingLotAvailabilityListService(
            reservationRepository,
            repository,
            options
        );

        var availability = (await service.GetDailyAvailabilityAsync(
            parkingLot.Id, DateOnly.FromDateTime(testDt)
        )).Unwrap();

        // assert
        AssertSlotReservedCounts(
            availability,
            expectedMaxCapacity: 3,
            (startTime, endTime) =>
            {
                if (startTime.Hour >= 4 && startTime.Hour < 8) return 1;
                if (startTime.Hour >= 8 && startTime.Hour < 16) return 2;
                if (startTime.Hour >= 16 && startTime.Hour < 20) return 1;
                return 0;
            }
        );
    }

    [Fact]
    public async Task Expected_ReservedCount_Per_Slot_With_MultiDay_And_OutOfRange_Reservations_Test()
    {
        // Arrange
        var (account, parkingLot, testDt) = await SeedAccountAndParkingLotAsync();
        var arrange = async () => {
            using var session = await CreateConnectionAsync();
            await session.BeginAsync();

            DataBaseTestReservationRepository reservationRepository =
            new DataBaseTestReservationRepository(session);

            var reservations = new[] {
                ReservationEntity.CreateNewReservation(
                    new ReservationNumberUnit("000000"),
                    account.Id,
                    parkingLot.Id,
                    testDt.AddDays(-1),
                    testDt.AddHours(12),
                    "BusCompany"
                ),
                ReservationEntity.CreateNewReservation(
                    new ReservationNumberUnit("000001"),
                    account.Id,
                    parkingLot.Id,
                    testDt.AddHours(12),
                    testDt.AddDays(1).AddHours(12),
                    "BusCompany"
                ),
                ReservationEntity.CreateNewReservation(
                    new ReservationNumberUnit("000002"),
                    account.Id,
                    parkingLot.Id,
                    testDt.AddDays(-1).AddHours(12),
                    testDt.AddDays(1).AddHours(12),
                    "BusCompany"
                )
            };

            foreach (var r in reservations)
            {
                await reservationRepository.AddAsync(r);
            }

            await session.CommitAsync();
        };

        await arrange();

        // Act
        using var session = await CreateConnectionAsync();
        IParkingLotRepository repository = new ParkingLotRepository(session);
        IReservationRepository reservationRepository = new ReservationRepository(session);

        var options = new ParkingLotAvailabilityOptions
        {
            SlotInterval = TimeSpan.FromMinutes(30)
        };

        IParkingLotAvailabilityListService service = new ParkingLotAvailabilityListService(
            reservationRepository,
            repository,
            options
        );

        var availability = (await service.GetDailyAvailabilityAsync(
            parkingLot.Id, DateOnly.FromDateTime(testDt)
        )).Unwrap();

        // Assert
        AssertSlotReservedCounts(
            availability,
            expectedMaxCapacity: 3,
            (startTime, endTime) => 2
        );
    }

    [Fact]
    public async Task Canceled_Reservation_Should_Not_Affect_ReservedCount_From_Availability ()
    {
        // Arrange
        var (account, parkingLot, testDt) = await SeedAccountAndParkingLotAsync();
        var arrange = async () => {
            using var session = await CreateConnectionAsync();
            await session.BeginAsync();

            DataBaseTestReservationRepository reservationRepository =
                new DataBaseTestReservationRepository(session);
            var reservation = ReservationEntity.CreateNewReservation(
                    new ReservationNumberUnit("000000"),
                    account.Id,
                    parkingLot.Id,
                    testDt,
                    testDt.AddHours(12),
                    "BusCompany"
                );
            reservation.Cancel();

            await reservationRepository.AddAsync(reservation);

            await session.CommitAsync();
        };

        await arrange();

        // Act
        using var session = await CreateConnectionAsync();
        IParkingLotRepository repository = new ParkingLotRepository(session);
        IReservationRepository reservationRepository = new ReservationRepository(session);

        var options = new ParkingLotAvailabilityOptions
        {
            SlotInterval = TimeSpan.FromMinutes(30)
        };

        IParkingLotAvailabilityListService service = new ParkingLotAvailabilityListService(
            reservationRepository,
            repository,
            options
        );

        var availability = (await service.GetDailyAvailabilityAsync(
            parkingLot.Id,
            DateOnly.FromDateTime(testDt)
        )).Unwrap();

        // Assert
        AssertSlotReservedCounts(
            availability,
            expectedMaxCapacity: 3,
            (startTime, endTime) => 0
        );
    }

    [Fact]
    public async Task When_ReservedCount_Is_Zero_Then_All_Slots_Should_Be_Zero_Reserved_Test()
    {
        // Arrange
        var (account, parkingLot, testDt) = await SeedAccountAndParkingLotAsync();

        using var session = await CreateConnectionAsync();

        IParkingLotRepository repository = new ParkingLotRepository(session);
        IReservationRepository reservationRepository = new ReservationRepository(session);

        var options = new ParkingLotAvailabilityOptions
        {
            SlotInterval = TimeSpan.FromMinutes(30)
        };

        IParkingLotAvailabilityListService service = new ParkingLotAvailabilityListService(
            reservationRepository,
            repository,
            options
        );

        // Act
        var availability = (await service.GetDailyAvailabilityAsync(parkingLot.Id, DateOnly.FromDateTime(testDt))).Unwrap();

        // Assert
        AssertSlotReservedCounts(
            availability,
            expectedMaxCapacity: 3,
            (startTime, endTime) => 0
        );
    }

    /// <summary>
    /// アベイラビリティスロットの予約台数が期待値と一致することを検証します。
    /// </summary>
    /// <param name="availability">アベイラビリティ情報</param>
    /// <param name="expectedMaxCapacity">期待される最大収容台数</param>
    /// <param name="getExpectedReservedCount">指定された時間帯の期待される予約台数を返す関数</param>
    private static void AssertSlotReservedCounts(
    ParkingLotAvailability availability,
    int expectedMaxCapacity,
    Func<DateTimeOffset, DateTimeOffset, int> getExpectedReservedCount)
    {
    foreach (var slot in availability.AvailabilitySlots)
    {
            var expectedReservedCount = getExpectedReservedCount(slot.StartTime, slot.EndTime);
            var actualReservedCount = slot.MaxReservationCapacity - slot.RemainingCapacity;
            slot.MaxReservationCapacity.ShouldBe(expectedMaxCapacity);
            actualReservedCount.ShouldBe(
                expectedReservedCount,
                $"Slot {slot.StartTime:yyyy-MM-dd HH:mm}-{slot.EndTime:HH:mm} の予約台数が期待値と一致しません");
            slot.RemainingCapacity.ShouldBe(expectedMaxCapacity - expectedReservedCount);
        }
    }


    /// <summary>
    /// テストのためのアカウントと駐車場情報、基準日時をセットアップします。
    /// </summary>
    /// <param name="maxCapacity">駐車場の最大収容台数</param>
    /// <param name="reservationCapacity">駐車場の予約可能台数</param>
    /// <param name="parkingLotName">駐車場の名前</param>
    /// <returns>作成されたアカウント、駐車場情報、および基準日時のタプル</returns>
    async Task<(AccountEntity, ParkingLotEntity, DateTime)> SeedAccountAndParkingLotAsync(
        short maxCapacity = 5,
        short reservationCapacity = 3,
        string parkingLotName = "Test Parking Lot")
    {
            using var session = await CreateConnectionAsync();
            await session.BeginAsync();

            DataBaseTestAccountRepository accountRepository =
                new DataBaseTestAccountRepository(session);
            DataBaseTestUserRepository userRepository =
                new DataBaseTestUserRepository(session);
            DataBaseTestParkingLotRepository parkingLotRepository =
                new DataBaseTestParkingLotRepository(session);
            DataBaseTestReservationRepository reservationRepository =
                new DataBaseTestReservationRepository(session);

            var a = AccountEntity.CreateUserRoleAccount(
                "test@example.com",
                System.Text.Encoding.UTF8.GetBytes("password"),
                System.Text.Encoding.UTF8.GetBytes("salt"));
            a.Activate();

            await accountRepository.AddAsync(a);
            await userRepository.AddAsync(a.Id, "Test", "User");

            var parkingLot = await parkingLotRepository.AddAsync(
                parkingLotName,
                maxCapacity : maxCapacity,
                reservationCapacity : reservationCapacity);

            var dt = new DateTime(2026, 1, 1, 0, 0, 0);

            await session.CommitAsync();

            return (a,parkingLot, dt);
    }
    
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}