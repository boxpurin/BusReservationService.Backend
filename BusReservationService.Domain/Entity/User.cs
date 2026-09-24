using BusReservationService.Domain.ValueObject;

namespace BusReservationService.Domain.Entity;

/// <summary>
/// 人そのもの
/// </summary>
public class UserEntity
{
    public Guid AccountId { get; private set; }
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string PhoneNumber { get; private set; } = null!;
    public UserType UserType { get; private set; }

    public UserEntity(Guid accountId , string firstName, string lastName, string phoneNumber, UserType userType)
    {
        AccountId = accountId;
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        UserType = userType;
    }
}
