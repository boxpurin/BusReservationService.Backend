using BusReservationService.Domain.Entity;
using BusReservationService.Domain.ValueObject.AccountRole;
using BusReservationService.Domain.ValueObject.AccountStatus;


namespace BusReservationService.Models;

public class AccountModel
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public AccountRole AccountRole { get; set; }
    public AccountStatus AccountStatus { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public DateTime? ActivatedAt { get; set; }
    public AccountModel() { }
    public AccountModel(AccountEntity entity)
    {
        Id = entity.Id;
        Email = entity.Email;
        AccountRole = entity.AccountRole;
        AccountStatus = entity.AccountStatus;
        LastLoginAt = entity.LastLoginAt;
        ActivatedAt = entity.ActivatedAt;
    }
}
