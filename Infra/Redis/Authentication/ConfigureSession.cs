using BusReservationService.Redis;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

namespace BusReservationService.Redis.Authentication;

public sealed class ConfigureCookieSessionStore(
    RedisTicketStore ticketStore)
    : IPostConfigureOptions<CookieAuthenticationOptions>
{
    public void PostConfigure(
        string? name,
        CookieAuthenticationOptions options)
    {
        if(name is not CookieAuthenticationDefaults.AuthenticationScheme)
        {
            return;
        }

        options.SessionStore = ticketStore;
    }
}