using FluentAssertions;
using BusReservationService.Domain.Repository;
using BusReservationService.Database.Repository;
using System.Data;
using BusReservationService.Domain.Entity;
using BusReservationService.Database.Service;
using Dapper;
using Microsoft.Extensions.Configuration;
using BusReservationService.Database.Test.Utils;
using Respawn;


namespace BusReservationService.Database.Test.Repositories;

public class AccountRepositoryTest : IClassFixture<DatabaseFixture>,IAsyncLifetime
{
    private readonly DatabaseFixture _fixture;
    private DatabaseService? dbService = null;
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

        result.Id.Should().NotBeEmpty();
        result.Email.Should().Be(account.Email);
        result.PasswordHash.Should().Equal(account.PasswordHash);
        result.PasswordSalt.Should().Equal(account.PasswordSalt);
        result.AccountStatus.Should().Be(account.AccountStatus);
        result.AccountRole.Should().Be(account.AccountRole);
        result.ActivatedAt.Should().BeNull();
        result.LastLoginAt.Should().BeNull();
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

        account.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAllAsync_Test()
    {
        IAccountRepository accountRepository = new AccountRepository(
            await CreateConnectionAsync(["Sql/Seeds/001_seeds.sql"])
        );

        var accounts = await accountRepository.GetAllAsync();

        accounts.Length.Should().Be(TestAccountIds.IDList.Length);
    }

    [Fact]
    public async Task If_Exist_Account_Then_FindByEmail_Should_NotBeNull()
    {
        IAccountRepository accountRepository = new AccountRepository(
            await CreateConnectionAsync(["Sql/Seeds/001_seeds.sql"])
        );

        var account = await accountRepository!.FindByEmailAsync("test1@example.com");
        account.Should().NotBeNull();
    }

    
    [Fact]
    public async Task If_Not_Exist_Account_Then_FindByEmail_Should_NotBeNull()
    {
        IAccountRepository accountRepository = new AccountRepository(
            await CreateConnectionAsync(["Sql/Seeds/001_seeds.sql"])
        );

        var account = await accountRepository!.FindByEmailAsync("not_exist_address@example.com");
        account.Should().BeNull();
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

        account.Should().NotBeNull();
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
        account.Should().BeNull();
    }

    [Fact]
    public async Task If_Exist_Account_Then_ExistByEmail_Should_True()
    {
        IAccountRepository accountRepository = new AccountRepository(
            await CreateConnectionAsync(["Sql/Seeds/001_seeds.sql"])
        );

        var ret = await accountRepository!.IsExistByEmailAsync("test1@example.com");
        ret.Should().Be(true);
    }

    
    [Fact]
    public async Task If_Not_Exist_Account_Then_ExistByEmail_Should_True()
    {
        IAccountRepository accountRepository = new AccountRepository(
            await CreateConnectionAsync(["Sql/Seeds/001_seeds.sql"])
        );

        var ret = await accountRepository!.IsExistByEmailAsync("not_exist_address@example.com");
        ret.Should().Be(false);
    }
    
    [Fact]
    public async Task If_Exist_Account_Then_ExistByAccountId_Should_True()
    {
        IAccountRepository accountRepository = new AccountRepository(
            await CreateConnectionAsync(["Sql/Seeds/001_seeds.sql"])
        );
        
        var ret = await accountRepository!.IsExistByAccountIdAsync(TestAccountIds.IDList.First());
        ret.Should().Be(true);
    }

    
    [Fact]
    public async Task If_Not_Exist_Account_Then_ExistByAccountId_Should_False()
    {
        IAccountRepository accountRepository = new AccountRepository(
            await CreateConnectionAsync(["Sql/Seeds/001_seeds.sql"])
        );
        
        var ret = await accountRepository!.IsExistByAccountIdAsync(TestAccountIds.NotExistId);
        ret.Should().Be(false);
    }

    [Fact]
    public async Task Delete_Account()
    {
        IAccountRepository accountRepository = new AccountRepository(
            await CreateConnectionAsync(["Sql/Seeds/001_seeds.sql"])
        );

        var account = await accountRepository!.FindByAccountIdAsync(TestAccountIds.IDList.First());

        bool deleted = await accountRepository.DeleteAsync(account!);
        deleted.Should().Be(true);
        
        deleted = await accountRepository.DeleteAsync(account!);
        deleted.Should().Be(false);

        account = await accountRepository!.FindByAccountIdAsync(TestAccountIds.IDList.First());
        account.Should().BeNull();
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
};