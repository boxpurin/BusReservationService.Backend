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

public class UserRepositoryTest : IClassFixture<DatabaseFixture>,IAsyncLifetime
{
    private readonly DatabaseFixture _fixture;
    private DatabaseService? dbService = null;
    private Respawner? _respawner = null;

    public AccountRepositoryTest(DatabaseFixture fixture)
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
            });
        }

        // 1. 全テーブルのデータを空にする
        await _respawner.ResetAsync((System.Data.Common.DbConnection)conn);

        // 2. テストが始まる前に、SeedデータをDapperで直接流し込んで復元する
        var seedSqlPath = Path.GetFullPath("Sql/Seeds/001_accounts.sql");
        var seedSql = await File.ReadAllTextAsync(seedSqlPath);
        await conn.ExecuteAsync(seedSql);
    }

    // IDbConnection を生成して返すファクトリメソッド
    public IDbConnection CreateConnection()
    {
        return dbService!.CreateConnection();
    }

    [Theory]
    [InlineData("password", "salt")]
    [InlineData("password1234", "salt")]
    [InlineData("1234password", "salt")]
    public async Task AddAsync_Test(string password, string salt)
    {
        IAccountRepository accountRepository = new AccountRepository(
            CreateConnection()
        );

        var account = AccountEntity.CreateUserRoleAccount(
            "test@email.com", 
            System.Text.Encoding.UTF8.GetBytes(password),
            System.Text.Encoding.UTF8.GetBytes(salt)
        );
    
        var result = await accountRepository.AddAsync(account);

        result.Id.Should().NotBeEmpty();
        result.Email.Should().Equals(account.Email);
        result.PasswordHash.Should().Equal(account.PasswordHash);
        result.PasswordSalt.Should().Equal(account.PasswordSalt);
        result.AccountStatus.Should().Equals(account.AccountStatus);
        result.AccountRole.Should().Equals(account.AccountRole);
        result.ActivatedAt.Should().BeNull();
        result.LastLoginAt.Should().BeNull();
    }

    [Fact]
    public async Task Update_Async_Test()
    {        
        IAccountRepository accountRepository = new AccountRepository(
            CreateConnection()
        );

        var account = await accountRepository.FindByEmailAsync("test1@example.com");
        
        account!.Activate();
        account = await accountRepository.UpdateAsync(account);

        account.Should().NotBeNull();
    }

    [Fact]
    public async Task If_Exist_Account_Then_FindByEmail_Should_NotBeNull()
    {
        IAccountRepository accountRepository = new AccountRepository(
            CreateConnection()
        );

        var account = await accountRepository!.FindByEmailAsync("test1@example.com");
        account.Should().NotBeNull();
    }

    
    [Fact]
    public async Task If_Not_Exist_Account_Then_FindByEmail_Should_NotBeNull()
    {
        IAccountRepository accountRepository = new AccountRepository(
            CreateConnection()
        );

        var account = await accountRepository!.FindByEmailAsync("not_exist_address@example.com");
        account.Should().BeNull();
    }

    
    [Fact]
    public async Task Delete_Account()
    {
        IAccountRepository accountRepository = new AccountRepository(
            CreateConnection()
        );

        var account = await accountRepository!.FindByEmailAsync("test1@example.com");

        var ret = await accountRepository.DeleteAsync(account!);
        ret.Should().Equals(1);
        
        ret = await accountRepository.DeleteAsync(account!);
        ret.Should().Equals(0);

        account = await accountRepository!.FindByEmailAsync("test1@example.com");
        account.Should().BeNull();
    }

    public Task DisposeAsync() => Task.CompletedTask;
};