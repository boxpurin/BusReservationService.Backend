using BusReservationService.Domain.ValueObject.AccountStatus;
using BusReservationService.Domain.ValueObject.AccountRole;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusReservationService.Domain.Entity;

public class AccountEntity
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = null!;
    public byte[] PasswordHash { get; private set; } = null!;
    public byte[] PasswordSalt { get; private set; } = null!;
    public AccountRole AccountRole { get; private set; }
    public AccountStatus AccountStatus { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public DateTime? ActivatedAt { get; private set; }

    public AccountEntity(Guid id , string email, byte[] passwordHash, byte[] salt, AccountRole role, AccountStatus status, DateTime? lastLoginAt = null, DateTime? activatedAt = null)
    {
        Id = id;
        Email = email;
        PasswordHash = passwordHash;
        PasswordSalt = salt;
        AccountRole = role;
        AccountStatus = status;
        LastLoginAt = lastLoginAt;
        ActivatedAt = activatedAt;
    }

    static public AccountEntity CreateUserRoleAccount(string email, byte[] passwordHash, byte[] salt)
    {
        return new AccountEntity
            (
                Guid.CreateVersion7(),
                email,
                passwordHash,
                salt,
                AccountRole.User,
                AccountStatus.JustCreated
            );
    }
    static public AccountEntity CreateAdminRoleAccount(string email, byte[] passwordHash, byte[] salt)
    {
        return new AccountEntity
            (
                Guid.CreateVersion7(),
                email,
                passwordHash,
                salt,
                AccountRole.Admin,
                AccountStatus.JustCreated
            );
    }

    /// <summary>
    /// アカウントステータスをメール確認待ちに設定する
    /// </summary>
    public void WaitForEmailVerification()
    {
        AccountStatus = AccountStatus.PendingEmailVerify;
    }

    /// <summary>
    /// アカウントステータスをオンボーディング中（プロフィール入力待ち）に設定する
    /// </summary>
    public void WaitForOnboard()
    {
        AccountStatus = AccountStatus.OnBoarding;
    }

    /// <summary>
    /// アカウントステータスを有効化済みに設定する
    /// メール認証、プロフィール入力が完了した状態
    /// また、アカウントが有効化された日時を記録する
    /// </summary>
    public void Activate()
    {
        AccountStatus = AccountStatus.Activated;
        ActivatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// アカウントステータスを停止状態に設定する
    /// 論理削除状態
    /// </summary>
    public void Suspend()
    {
        AccountStatus = AccountStatus.Suspended;
    }
}