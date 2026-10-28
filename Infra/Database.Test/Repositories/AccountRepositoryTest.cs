using BusReservationService.Domain.Repository;
using BusReservationService.Database.Repository;
using System.Data;
using BusReservationService.Domain.Entity;
using BusReservationService.Database.Service;
using Shouldly;
using Dapper;
using Microsoft.Extensions.Configuration;
using BusReservationService.Database.Test.Utils;
using Respawn;
using BusReservationService.Services.Interface;


namespace BusReservationService.Database.Test.Repositories;

public class AccountRepositoryTest : IClassFixture<DatabaseFixture>,IAsyncLifetime
{
    private readonly DatabaseFixture _fixture;
    private IDatabaseService? dbService = null;
    private Respawner? _respawner = null;

    public AccountRepositoryTest(DatabaseFixture fixture)
    {
        _fixture = fixture;
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

    [Theory]
    [InlineData("password", "salt")]
    [InlineData("password1234", "salt")]
    [InlineData("1234password", "salt")]
    public async Task AddAsync_Test(string password, string salt)
    {
        IAccountRepository accountRepository = new AccountRepository(
            await CreateConnectionAsync()
        );

        var account = AccountEntity.CreateUserRoleAccount(
            "test@email.com", 
            System.Text.Encoding.UTF8.GetBytes(password),
            System.Text.Encoding.UTF8.GetBytes(salt)
        );
    
        var result = await accountRepository.AddAsync(account);

        result.Id.ShouldNotBe(Guid.Empty);
        result.Email.ShouldBe(account.Email);
        result.PasswordHash.ShouldBe(account.PasswordHash);
        result.PasswordSalt.ShouldBe(account.PasswordSalt);
        result.AccountStatus.ShouldBe(account.AccountStatus);
        result.AccountRole.ShouldBe(account.AccountRole);
        result.ActivatedAt.ShouldBeNull();
        result.LastLoginAt.ShouldBeNull();
    }

    [Fact]
    public async Task Update_Async_Test()
    {        
        IAccountRepository accountRepository = new AccountRepository(
            await CreateConnectionAsync(["Sql/Seeds/001_seeds.sql"])
        );

        var account = await accountRepository.FindByEmailAsync("test1@example.com");
        
        account!.Activate();
        account = await accountRepository.UpdateAsync(account);

        account.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetAllAsync_Test()
    {
        IAccountRepository accountRepository = new AccountRepository(
            await CreateConnectionAsync(["Sql/Seeds/001_seeds.sql"])
        );

        var accounts = await accountRepository.GetAllAsync();

        accounts.Length.ShouldBe(TestAccountIds.IDList.Length);
    }

    [Fact]
    public async Task If_Exist_Account_Then_FindByEmail_Should_NotBeNull()
    {
        IAccountRepository accountRepository = new AccountRepository(
            await CreateConnectionAsync(["Sql/Seeds/001_seeds.sql"])
        );

        var account = await accountRepository!.FindByEmailAsync("test1@example.com");
        account.ShouldNotBeNull();
    }

    
    [Fact]
    public async Task If_Not_Exist_Account_Then_FindByEmail_Should_NotBeNull()
    {
        IAccountRepository accountRepository = new AccountRepository(
            await CreateConnectionAsync(["Sql/Seeds/001_seeds.sql"])
        );

        var account = await accountRepository!.FindByEmailAsync("not_exist_address@example.com");
        account.ShouldBeNull();
    }
    
    [Fact]
    public async Task If_Exist_Account_Then_FindByAccountId_Should_NotBeNull()
    {
        IAccountRepository accountRepository = new AccountRepository(
            await CreateConnectionAsync(["Sql/Seeds/001_seeds.sql"])
        );
        
        var account = await accountRepository!.FindByAccountIdAsync(
            TestAccountIds.IDList.First()
        );

        account.ShouldNotBeNull();
    }

    [Fact]
    public async Task If_Not_Exist_Account_Then_FindByAccountId_Should_NotBeNull()
    {
        IAccountRepository accountRepository = new AccountRepository(
            await CreateConnectionAsync(["Sql/Seeds/001_seeds.sql"])
        );

        // AccountIdはGUID v7
        var account = await accountRepository!.FindByAccountIdAsync(
            TestAccountIds.NotExistId
        );
        account.ShouldBeNull();
    }

    [Fact]
    public async Task If_Exist_Account_Then_ExistByEmail_Should_True()
    {
        IAccountRepository accountRepository = new AccountRepository(
            await CreateConnectionAsync(["Sql/Seeds/001_seeds.sql"])
        );

        var ret = await accountRepository!.IsExistByEmailAsync("test1@example.com");
        ret.ShouldBe(true);
    }

    
    [Fact]
    public async Task If_Not_Exist_Account_Then_ExistByEmail_Should_True()
    {
        IAccountRepository accountRepository = new AccountRepository(
            await CreateConnectionAsync(["Sql/Seeds/001_seeds.sql"])
        );

        var ret = await accountRepository!.IsExistByEmailAsync("not_exist_address@example.com");
        ret.ShouldBe(false);
    }
    
    [Fact]
    public async Task If_Exist_Account_Then_ExistByAccountId_Should_True()
    {
        IAccountRepository accountRepository = new AccountRepository(
            await CreateConnectionAsync(["Sql/Seeds/001_seeds.sql"])
        );
        
        var ret = await accountRepository!.IsExistByAccountIdAsync(TestAccountIds.IDList.First());
        ret.ShouldBe(true);
    }

    
    [Fact]
    public async Task If_Not_Exist_Account_Then_ExistByAccountId_Should_False()
    {
        IAccountRepository accountRepository = new AccountRepository(
            await CreateConnectionAsync(["Sql/Seeds/001_seeds.sql"])
        );
        
        var ret = await accountRepository!.IsExistByAccountIdAsync(TestAccountIds.NotExistId);
        ret.ShouldBe(false);
    }

    [Fact]
    public async Task Delete_Account()
    {
        IAccountRepository accountRepository = new AccountRepository(
            await CreateConnectionAsync(["Sql/Seeds/001_seeds.sql"])
        );

        var account = await accountRepository!.FindByAccountIdAsync(TestAccountIds.IDList.First());

        bool deleted = await accountRepository.DeleteAsync(account!);
        deleted.ShouldBe(true);
        
        deleted = await accountRepository.DeleteAsync(account!);
        deleted.ShouldBe(false);

        account = await accountRepository!.FindByAccountIdAsync(TestAccountIds.IDList.First());
        account.ShouldBeNull();
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
};