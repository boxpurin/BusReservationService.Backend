using BusReservationService.Database.Enum;
using BusReservationService.Domain.Entity;


namespace BusReservationService.Database.Table;


public class AccountTable
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public AccountRole AccountRole { get; set; }
    public AccountStatus AccountStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? ActivatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }

    public AccountEntity ToEntity()
    {
        var role = AccountRole;

        return new AccountEntity(
            Id,
            Email,
            Password,
            AccountRole.ToDomain(),
            AccountStatus.ToDomain(),
            LastLoginAt
        );
    }
}
