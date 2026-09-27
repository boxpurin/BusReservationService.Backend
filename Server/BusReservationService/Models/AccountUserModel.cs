using BusReservationService.Domain.ValueObject;

namespace BusReservationService.Models;
public class AccountUserModel
{    
    public Guid AccountId { get; set; }
    public string Email { get; set; } = string.Empty;
    public byte[] PasswordHash { get; set; } = [];
    public byte[] PasswordSalt { get; set; } = [];
    
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public AccountRole AccountRole { get; set; }
    public AccountStatus AccountStatus { get; set; }
    public UserType UserType { get; set; }

    public AccountUserModel()
    {}
}
