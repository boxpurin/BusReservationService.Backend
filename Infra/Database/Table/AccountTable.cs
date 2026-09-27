using BusReservationService.Database.Enums;
using BusReservationService.Domain.Entity;
using BusReservationService.Share;


namespace BusReservationService.Database.Table;


public class AccountTable : IConvertibleToDomain<AccountEntity>
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public byte[] PasswordHash { get; set; } = [];
    public byte[] PasswordSalt { get; set; } = [];
    public AccountRole AccountRole { get; set; }
    public AccountStatus AccountStatus { get; set; }
    public DateTime? ActivatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public AccountEntity ToDomain()
    {
        return new AccountEntity(
            Id,
            Email,
            PasswordHash,
            PasswordSalt,
            AccountRole.ToDomain(),
            AccountStatus.ToDomain(),
            LastLoginAt
        );
    }
}
