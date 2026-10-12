using BusReservationService.Domain.ValueObject;
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

    public void Activate()
    {
        AccountStatus = AccountStatus.Activated;
        ActivatedAt = DateTime.UtcNow;
    }

    public void Suspend()
    {
        AccountStatus = AccountStatus.Suspended;
    }
}