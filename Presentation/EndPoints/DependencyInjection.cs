using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BusReservationService.Endpoints;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentationEndpoints(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
        .AddAuthenticationCookie(validFor: TimeSpan.FromMinutes(3), options: options =>
        {
            options.Cookie.Name = "__Host-BusReservationService.SessionId";
        }) //configure cookie auth
        .AddAuthorization(options =>
        {
            options.AddPolicy("VerifiedUser", x => x.RequireRole("User").RequireClaim("SessionId"));
            options.AddPolicy("AdminOnly", x => x.RequireRole("Admin").RequireClaim("SessionId"));
        }) //add this
        .AddFastEndpoints();

        return services;
    }
}
