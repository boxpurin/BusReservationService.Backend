using System.Data;
using BusReservationService.Database.Repository;
using BusReservationService.Database.Service;
using BusReservationService.Database.Test.Utils;
using BusReservationService.Domain.Entity;
using BusReservationService.Domain.Repository;
using Dapper;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Respawn;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace BusReservationService.Database.Test.Repositories;

public class UserRepositoryTest : IClassFixture<DatabaseFixture>, IAsyncLifetime
{
    private readonly DatabaseFixture _fixture;
    private DatabaseService? dbService = null;
    private Respawner? _respawner = null;

    public UserRepositoryTest(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task InitializeAsync()
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
    

    // IDbConnection を生成して返すファクトリメソッド
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
    public async Task Add_User_Exist_Account()
    {
        IUserRepository userRepository = new UserRepository(
            await CreateConnectionAsync(["Sql/Seeds/001_seeds.sql"])
        );

        var dto = new AddAsymcDto(
            TestAccountIds.IDList[1],
            "First",
            "Last",
            "0000000",
            "東京都千代田区1-1-1",
            "090-0000-0000",
            null,
            Domain.ValueObject.UserType.Personal
        );

        var user = await userRepository.AddAsymc(dto);

        user.Should().NotBeNull();
    }
    
    [Fact]
    public async Task Add_User_Not_Exist_Account()
    {
        IUserRepository userRepository = new UserRepository(
            await CreateConnectionAsync()
        );

        var dto = new AddAsymcDto(
            TestAccountIds.NotExistId,
            "First",
            "Last",
            "0000000",
            "東京都千代田区1-1-1",
            "090-0000-0000",
            null,
            Domain.ValueObject.UserType.Personal
        );

        Func<Task> act = async () => await userRepository.AddAsymc(dto);

        await act.Should().ThrowAsync<Npgsql.PostgresException>();
    }

    [Fact]
    public async Task UpdateUser()
    {
        IUserRepository userRepository = new UserRepository(
            await CreateConnectionAsync(["Sql/Seeds/001_seeds.sql"])
        );

        var prev = TestUsers.ExistPersonalUser;

        var expect = UserEntity.CreateUserEntity(
            prev.AccountId,
            "ExpectedFirst",
            "ExpectedLast",
            "0000000",
            "東京都千代田区1-1-1",
            "070-0000-1234",
            "07-1234-5678",
            Domain.ValueObject.UserType.Personal
        );

        var ret = await userRepository.UpdateAsync(expect);

        ret.Should().NotBeNull();

        ret.Should().BeEquivalentTo(expect);
    }

    [Fact]
    public async Task FindByAccountId()
    {        
        IUserRepository userRepository = new UserRepository(
            await CreateConnectionAsync(["Sql/Seeds/001_seeds.sql"])
        );

        var user = await userRepository.FindByAccountIdAsync(
            TestUsers.ExistPersonalUser.AccountId
        );
        user.Should().NotBeNull();

        user = await userRepository.FindByAccountIdAsync(
            TestAccountIds.NotExistId
        );
        user.Should().BeNull();
    }

    [Fact]
    public async Task ExistByAccountId()
    {
        IUserRepository userRepository = new UserRepository(
            await CreateConnectionAsync(["Sql/Seeds/001_seeds.sql"])
        );

        var user = await userRepository.ExistByAccountIdAsync(
            TestUsers.ExistPersonalUser.AccountId
        );
        user.Should().Be(true);
        
        user = await userRepository.ExistByAccountIdAsync(
            TestUsers.NotExistId
        );
        user.Should().Be(false);
    }

    
    [Fact]
    public async Task DeleteUser()
    {
        IUserRepository userRepository = new UserRepository(
            await CreateConnectionAsync(["Sql/Seeds/001_seeds.sql"])
        );

        var deleted = await userRepository.DeleteByAccountIdAsync(
            TestUsers.ExistPersonalUser.AccountId
        );

        deleted.Should().Be(true);
        
        deleted = await userRepository.DeleteByAccountIdAsync(
            TestUsers.ExistPersonalUser.AccountId
        );

        deleted.Should().Be(false);

    }


    public Task DisposeAsync() => Task.CompletedTask;
}