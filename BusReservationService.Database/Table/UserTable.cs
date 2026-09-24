using BusReservationService.Domain.Entity;

using UserTypeDomain = BusReservationService.Domain.ValueObject.UserType;

namespace BusReservationService.Database.Table;

public enum UserType
{
    // 個人顧客
    Personal,
    // 法人顧客
    Business,
    // 管理者
    Administrator,
}
internal static class UserTypeExtension
{
    internal static UserTypeDomain ToDomain(this UserType userType)
    {
        return userType switch
        {
            UserType.Personal => UserTypeDomain.Personal,
            UserType.Business => UserTypeDomain.Business,
            UserType.Administrator => UserTypeDomain.Administrator,
            _ => throw new ApplicationException()
        };
    }
}

/// <summary>
/// 人そのもの
/// </summary>
public class UserTable
{
    public Guid AccountId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public UserType UserType { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public UserTable()
    {

    }

    public UserEntity ToEntity()
    {
        return new UserEntity(
            AccountId,
            FirstName,
            LastName,
            PhoneNumber,
            UserType.ToDomain()
        );
    }
}
