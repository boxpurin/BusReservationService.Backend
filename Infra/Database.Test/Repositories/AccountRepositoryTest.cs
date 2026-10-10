using Xunit;
using FluentAssertions;
using Testcontainers.Xunit;
using Testcontainers.PostgreSql;
using Xunit.Abstractions;
using BusReservationService.Domain.Repository;
using BusReservationService.Database.Repository;
using System.Data;
using BusReservationService.Domain.Entity;
using DotNet.Testcontainers.Configurations;
using BusReservationService.Database.Service;
using Microsoft.Extensions.Configuration;


namespace BusReservationService.Database.Test.Repositories;

public class UserRepositoryTest : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = 
    new PostgreSqlBuilder("postgres:16-alpine")
    .WithDatabase("test_db")
    .WithUsername("postgres")
    .WithPassword("password")
    .WithBindMount(
        Path.GetFullPath("../../../Infra/Database/Migrations/20260911084734_init.sql"), 
        "/docker-entrypoint-initdb.d/01_init.sql", 
        AccessMode.ReadOnly)
    .WithBindMount(
        Path.GetFullPath("../../../Infra/Database.Test/Sql/Seeds/001_accounts.sql"), 
        "/docker-entrypoint-initdb.d/02_accounts.sql", 
        AccessMode.ReadOnly)
    .Build();

    private DatabaseService? dbService = null;


    public async Task InitializeAsync()
    {        
        await _postgres.StartAsync(); 
        // 2. Get the dynamic connection string
        string connectionString = _postgres.GetConnectionString();

        // 3. Build IConfiguration with the container's connection string
        var inMemorySettings = new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] = connectionString
        };

        IConfiguration _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
            
        dbService = new DatabaseService(_configuration);
        dbService.ConfigureMappings();
    }

    // IDbConnection を生成して返すファクトリメソッド
    public IDbConnection CreateConnection()
    {
        return dbService!.CreateConnection();
    }

    [Fact]
    public async Task AddAsync_Test()
    {
        IAccountRepository accountRepository = new AccountRepository(
            CreateConnection()
        );

        var account = AccountEntity.CreateUserRoleAccount(
            "test@email.com", 
            System.Text.Encoding.UTF8.GetBytes("password"),
            System.Text.Encoding.UTF8.GetBytes("salt")
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
    public async Task If_Exist_Account_Then_FindByEmail_Should_NotBeNull()
    {
        IAccountRepository accountRepository = new AccountRepository(
            CreateConnection()
        );

        var account = await accountRepository!.FindByEmail("test1@example.com");
        account.Should().NotBeNull();
    }

    public Task DisposeAsync()
    {
        return _postgres.DisposeAsync().AsTask();
    }    
};