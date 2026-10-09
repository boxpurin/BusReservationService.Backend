using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace BusReservationService.Redis;
public sealed class RedisTicketStore(IConnectionMultiplexer redis) : ITicketStore
{
    private readonly string KeyPrefix = "auth:session:";
    private readonly IDatabase _db = redis.GetDatabase();
    public async Task<string> StoreAsync(AuthenticationTicket ticket)
    {
        return await StoreAsync(
            ticket,
            CancellationToken.None);
    }
    
    public async Task<string> StoreAsync(
        AuthenticationTicket ticket, 
        HttpContext context,
        CancellationToken cancellationToken)
    {
        return await StoreAsync(
            ticket,
            cancellationToken);
    }

    public async Task<string> StoreAsync(
        AuthenticationTicket ticket,
        CancellationToken cancellationToken)
    {
        var key = Guid.NewGuid().ToString("N");
        await StoreTicketAsync(key, ticket, cancellationToken);

        return key;
    }

    public async Task RenewAsync(
        string key,
        AuthenticationTicket ticket)
    {
        // Redis更新
        await RenewAsync(key, ticket, CancellationToken.None);
    }

    public async Task RenewAsync(
        string key,
        AuthenticationTicket ticket,
        CancellationToken cancellationToken)
    {
        // Redis更新
        await StoreTicketAsync(key, ticket, cancellationToken);
    }

    public async Task RenewAsync(
        string key,
        AuthenticationTicket ticket,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        await StoreTicketAsync(key, ticket, cancellationToken);
    }   

    public async Task<AuthenticationTicket?> RetrieveAsync(string key)
    {
        // Redisから取得
        var value = await _db.StringGetAsync(KeyPrefix + key);
        
        if (value.IsNullOrEmpty)
        {
            return null;
        }

        return TicketSerializer.Default.Deserialize(value!);
    }
    public async Task<AuthenticationTicket?> RetrieveAsync(
        string key,
        CancellationToken cancellationToken)
    {
        var data = await _db.StringGetAsync(KeyPrefix + key);

        if (data.IsNullOrEmpty)
        {
            return null;
        }

        return TicketSerializer.Default.Deserialize(data!);
    }

    public async Task RemoveAsync(string key)
    {
        // Redisから削除
        await RemoveAsync(key, CancellationToken.None);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken)
    {
        // Redisから削除
        await _db.KeyDeleteAsync(KeyPrefix + key);
    }

    public async Task RemoveAsync(
        string key,
        HttpContext context,
        CancellationToken cancellationToken)
    {
        await RemoveAsync(key, cancellationToken);
    }


    private async Task StoreTicketAsync(
        string key,
        AuthenticationTicket ticket,
        CancellationToken cancellationToken)
    {
        var data =
            TicketSerializer.Default.Serialize(ticket);
        
        TimeSpan? expiry = null;

        if (ticket.Properties.ExpiresUtc is { } expiresUtc)
        {
            expiry = expiresUtc - DateTimeOffset.UtcNow;

            if( expiry <= TimeSpan.Zero)
            {
                return;
            }
        }


        await _db.StringSetAsync(
            KeyPrefix + key,
            data,
            expiry,
            When.Always);
    }
}