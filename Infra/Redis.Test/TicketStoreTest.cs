using Xunit;
using Shouldly;
using Moq;
using Testcontainers.Redis;
using Microsoft.AspNetCore.Authentication;
using StackExchange.Redis;
using System.Security.Claims;
using BusReservationService.Redis;

namespace BusReservationService.Redis.Test;

/// <summary>
/// Testcontainersを使用したRedisコンテナとの統合テスト
/// </summary>
public class RedisTicketStoreIntegrationTests  : IAsyncLifetime
{
    private RedisContainer? _redisContainer;
    private IConnectionMultiplexer? _redis;
    private RedisTicketStore? _store;

    // テスト開始時に Redis コンテナを起動
    public async ValueTask InitializeAsync()
    {
        _redisContainer = new RedisBuilder("redis:7-alpine").Build();
        await _redisContainer.StartAsync();

        _redis = await ConnectionMultiplexer.ConnectAsync(
            _redisContainer.GetConnectionString()
        );

        _store = new RedisTicketStore(_redis);
    }

    [Fact]
    public async Task Store_And_Retrieve_Should_Match_Exactly_With_Real_Redis()
    {
        // 1. Arrange
        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "RealRedisUser") }, "Auth");
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), "Scheme");

        // 2. Act
        var key = await _store!.StoreAsync(ticket,TestContext.Current.CancellationToken);
        var retrieved = await _store.RetrieveAsync(key, TestContext.Current.CancellationToken);

        // 3. Assert
        retrieved.ShouldNotBeNull();
        retrieved!.Principal.Identity!.Name.ShouldBe("RealRedisUser");
    }

    [Fact]
    public async Task Ticket_Should_Expire_Based_On_Expiry_Date()
    {
        // Arrange: 2秒後に切れるチケットを作成
        var identity = new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.Name, "ExpiringUser") }, "Auth"
        );
        var properties = new AuthenticationProperties();
        properties.ExpiresUtc = DateTimeOffset.UtcNow.AddSeconds(2);
        var ticket = new AuthenticationTicket(
            new ClaimsPrincipal(identity), properties, "Scheme"
        );

        // Act
        var key = await _store!.StoreAsync(ticket, TestContext.Current.CancellationToken);
        
        // 直後は取得できる
        (await _store.RetrieveAsync(key, TestContext.Current.CancellationToken)).ShouldNotBeNull();

        // 2.5秒待つ
        await Task.Delay(2500, TestContext.Current.CancellationToken);

        // Assert: Redis の TTL で自動消滅していることを検証
        (await _store.RetrieveAsync(key, TestContext.Current.CancellationToken)).ShouldBeNull();
    }

    
    [Fact]
    public async Task ExpiresUtc_Should_Be_After_DateTime()
    {
        // Arrange: 2秒前に切れるチケットを作成
        var identity = new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.Name, "ExpiringUser") }, "Auth"
        );
        var properties = new AuthenticationProperties();
        properties.ExpiresUtc = DateTimeOffset.UtcNow.AddSeconds(-2);
        var ticket = new AuthenticationTicket(
            new ClaimsPrincipal(identity), properties, "Scheme"
        );

        // Act
        var key = await _store!.StoreAsync(ticket, TestContext.Current.CancellationToken);
        
        // 期限切れのチケットは追加されていない
        (await _store.RetrieveAsync(key, TestContext.Current.CancellationToken)).ShouldBeNull();
    }


    // テスト終了時にコンテナを破棄
    public async ValueTask DisposeAsync()
    {
        if (_redis != null) await _redis.DisposeAsync();
        if (_redisContainer != null) await _redisContainer.DisposeAsync();
    }
}