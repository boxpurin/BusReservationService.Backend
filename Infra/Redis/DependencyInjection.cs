using BusReservationService.Redis.Authentication;
using BusReservationService.Stores.Interface;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace BusReservationService.Redis;

public static class DependencyInjection
{
    public static IServiceCollection AddServiceSessionStore(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            var connectionString =
                configuration.GetConnectionString("Redis")
                ?? throw new InvalidOperationException(
                    "Redis connection string is not configured.");

            return ConnectionMultiplexer.Connect(connectionString);
        });

        services.AddSingleton<RedisTicketStore>();


        services.AddSingleton<
            IPostConfigureOptions<CookieAuthenticationOptions>,
            ConfigureCookieSessionStore>();

        services.AddOptions<ActivationSessionOptions>();
        services.AddSingleton<IActivationSessionStore,ActivationSessionStore>();

        return services;
    }
}