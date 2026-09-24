using BusReservationService.Domain.Entity;
using BusReservationService.Domain.ValueObject;


namespace BusReservationService.Models;

public class AccountModel
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public AccountRole AccountRole { get; set; }
    public AccountStatus AccountStatus { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime? ActivatedAt { get; set; }

    public AccountModel(AccountEntity entity)
    {
        Id = entity.Id;
        Email = entity.Email;
        Password = entity.Password;
        AccountRole = AccountRole;
        AccountStatus = AccountStatus;
        LastLoginAt = entity.LastLoginAt;
    }
}
