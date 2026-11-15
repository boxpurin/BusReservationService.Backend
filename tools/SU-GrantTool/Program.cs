using ConsoleAppFramework;
using BusReservationService.Services.Interface;
using BusReservationService.Database.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Dapper;
using System.Data;
using BusReservationService.Domain.Repository;
using BusReservationService.Database.Repository;
using BusReservationService.Domain.Entity;
using BusReservationService.Util;
using BusReservationService.Domain.ValueObject.AccountRole;

var app = ConsoleApp
    .Create()
    .ConfigureDefaultConfiguration()
    .ConfigureServices((configuration, services) =>
    {
        // Package Import: Microsoft.Extensions.Options.ConfigurationExtensions
        services.AddSingleton<IConfiguration>(configuration);
        services.AddSingleton<IDatabaseService, DatabaseService>();
        
        services.AddScoped<IDbConnection>(sp =>
        {
            var factory =
                sp.GetRequiredService<IDatabaseService>();

            return factory.CreateConnection();
        });

        services.AddScoped<IAccountRepository, AccountRepository>();
    });

    
await app.RunAsync(args);


[RegisterCommands("admin")]
class SuperuserCommands(IDbConnection conn, IAccountRepository accountRepository)
{
    /// <summary>Add new administrator account.</summary>
    [Command("add")]
    public async Task Add([Argument]string email, [Argument]string password)
    {
        Console.WriteLine($"Adding administrator with email: {email}");
            conn.Open();

            try{
                var (passwordHash, salt) = Argon2Helper.HashPassword(password);

                var account = AccountEntity.CreateAdminRoleAccount(email, passwordHash, salt);
                await accountRepository.AddAsync(account);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding administrator: {ex.Message}");
            }
    }

    /// <summary>List all administrator accounts.</summary>
    [Command("list")]
    public async Task List()
    {
        conn.Open();
        var accounts = (await accountRepository.GetAllAsync())
            .Where(a => a.AccountRole.IsSystemAuthority());

        foreach (var account in accounts)
        {
            Console.WriteLine($"ID: {account.Id}, Email: {account.Email}");
        }

    }
    
    /// <summary>Remove an administrator account.</summary>
    [Command("remove")]
    public async Task Remove([Argument] string email)
    {
        Console.WriteLine($"Removing administrator with email: {email}");
        conn.Open();

        try
        {
            var account = (await accountRepository.GetAllAsync())
                .FirstOrDefault(a => a.Email == email && a.AccountRole.IsSystemAuthority());

            if (account != null)
            {
                await accountRepository.DeleteAsync(account);
                Console.WriteLine($"Administrator with email: {email} removed successfully.");
            }
            else
            {
                Console.WriteLine($"Administrator with email: {email} not found.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error removing administrator: {ex.Message}");
        }
    }
}

public class SuperuserGrantToolOptions
{
    public string ConnectionString { get; set; } = string.Empty;
}