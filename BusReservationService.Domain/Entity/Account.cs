using BusReservationService.Domain.ValueObject;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusReservationService.Domain.Entity;

public class AccountEntity
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = null!;
    public string Password { get; private set; } = null!;
    public AccountRole AccountRole { get; private set; }
    public AccountStatus AccountStatus { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public DateTime? ActivatedAt { get; private set; }

    public AccountEntity(Guid id , string email, string password, AccountRole role, AccountStatus status, DateTime? lastLoginAt = null, DateTime? activatedAt = null)
    {
        Id = id;
        Email = email;
        Password = password;
        AccountRole = role;
        AccountStatus = status;
        LastLoginAt = lastLoginAt;
        ActivatedAt = activatedAt;
    }

    static public AccountEntity CreateUserRoleAccount(string email, string password)
    {
        return new AccountEntity
            (
                Guid.CreateVersion7(),
                email,
                password,
                AccountRole.User,
                AccountStatus.JustCreated
            );
    }

    public void Activate()
    {
        AccountStatus = AccountStatus.Active;
        ActivatedAt = DateTime.UtcNow;
    }

    public void Suspend()
    {
        AccountStatus = AccountStatus.Suspended;
    }
}