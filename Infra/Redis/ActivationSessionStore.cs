using System;
using System.Security.Cryptography;
using BusReservationService.Stores.Interface;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace BusReservationService.Redis;

public class ActivationSessionStore : IActivationSessionStore
{
    private const string KeyPrefix = "activation:session:";
    private readonly IDatabase _db;
    private readonly TimeSpan _lifetime;

    public ActivationSessionStore(
        IConnectionMultiplexer redis,
        IOptions<ActivationSessionOptions> options
    )
    {
        _db = redis.GetDatabase();
        _lifetime = options.Value.Lifetime;

        if (_lifetime <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(options),
                "有効期間は正の値にしてください。");
        }
    }

    public async Task<string> CreateAsync(Guid accountId, CancellationToken ct)
    {
        if (accountId == Guid.Empty)
        {
            throw new ArgumentException(
                "AccountIdが必要です。", nameof(accountId));
        }
        
        for (var attempt = 0; attempt < 3; attempt++)
        {
            ct.ThrowIfCancellationRequested();

            var sessionId = GenerateSessionId();

            var created = await _db.StringSetAsync(
                KeyPrefix + sessionId,
                accountId.ToString("D"),
                expiry: _lifetime,
                when: When.NotExists
            ).WaitAsync(ct);

            if (created)
            {
                return sessionId;
            }
        }

        throw new InvalidOperationException("登録用セッションの発行に失敗しました。");
    }

    public async Task DeleteAsync(
        string sessionId,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if (!IsValidSessionId(sessionId))
        {
            return;
        }

        await _db.KeyDeleteAsync(
            KeyPrefix + sessionId
        ).WaitAsync(ct);
    }

    public async Task<Guid> FindAccountIdAsync(
        string sessionId,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        if (!IsValidSessionId(sessionId))
        {
            return Guid.Empty;
        }

        var value = await _db.StringGetAsync(
            KeyPrefix + sessionId
        ).WaitAsync(ct);

        if (value.IsNullOrEmpty)
        {
            return Guid.Empty;
        }

        if (Guid.TryParseExact(value.ToString(), "D", out var accountId)
            && accountId != Guid.Empty)
        {
            return accountId;
        }

        throw new InvalidOperationException(
            "登録用セッションの保存データが不正です。");
    }

    private static string GenerateSessionId()
    {
        return Convert.ToBase64String(
                RandomNumberGenerator.GetBytes(32))
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private static bool IsValidSessionId(string? sessionId)
    {
        // 32バイトのBase64URL表現は、パディングなしで43文字。
        return sessionId is { Length: 43 }
            && sessionId.All(c =>
                char.IsAsciiLetterOrDigit(c) || c is '-' or '_');
    }
}
