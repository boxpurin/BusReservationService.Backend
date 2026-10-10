using BusReservationService.Redis;
using BusReservationService.Redis.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Moq;
using StackExchange.Redis;
using Xunit;

namespace BusReservationService.Redis.Test;

public class ConfigureCookieSessionStoreTests
{
    private static RedisTicketStore CreateTicketStore()
    {
        var database = new Mock<IDatabase>(MockBehavior.Strict);
        var redis = new Mock<IConnectionMultiplexer>(MockBehavior.Strict);

        // RedisTicketStore のコンストラクターで呼ばれるため設定する。
        // Moq の式では省略可能な引数も明示する。
        redis.Setup(x => x.GetDatabase(-1, null))
            .Returns(database.Object);

        return new RedisTicketStore(redis.Object);
    }

    [Fact]
    public void PostConfigure_CookieScheme_SetsSessionStore()
    {
        // Arrange
        var ticketStore = CreateTicketStore();
        var sut = new ConfigureCookieSessionStore(ticketStore);
        var options = new CookieAuthenticationOptions();

        // Act
        sut.PostConfigure(
            CookieAuthenticationDefaults.AuthenticationScheme,
            options);

        // Assert: 渡したインスタンスそのものが設定される
        Assert.Same(ticketStore, options.SessionStore);
    }

    [Fact]
    public void PostConfigure_CookieScheme_ReplacesExistingSessionStore()
    {
        var ticketStore = CreateTicketStore();
        var existingStore = new Mock<ITicketStore>().Object;
        var sut = new ConfigureCookieSessionStore(ticketStore);
        var options = new CookieAuthenticationOptions
        {
            SessionStore = existingStore
        };

        sut.PostConfigure(
            CookieAuthenticationDefaults.AuthenticationScheme,
            options);

        Assert.Same(ticketStore, options.SessionStore);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("OtherScheme")]
    [InlineData("cookies")]
    [InlineData("COOKIES")]
    public void PostConfigure_OtherScheme_LeavesSessionStoreNull(
        string? name)
    {
        var sut = new ConfigureCookieSessionStore(CreateTicketStore());
        var options = new CookieAuthenticationOptions();

        sut.PostConfigure(name, options);

        Assert.Null(options.SessionStore);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("OtherScheme")]
    [InlineData("cookies")]
    [InlineData("COOKIES")]
    public void PostConfigure_OtherScheme_PreservesExistingSessionStore(
        string? name)
    {
        var existingStore = new Mock<ITicketStore>().Object;
        var sut = new ConfigureCookieSessionStore(CreateTicketStore());
        var options = new CookieAuthenticationOptions
        {
            SessionStore = existingStore
        };

        sut.PostConfigure(name, options);

        Assert.Same(existingStore, options.SessionStore);
    }
}