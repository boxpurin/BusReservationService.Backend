using UserTypeDomain = BusReservationService.Domain.ValueObject.UserType;
namespace BusReservationService.Database.Enums;

internal enum UserType
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
    public static UserTypeDomain ToDomain(this UserType userType) => 
    Enum.Parse<UserTypeDomain>(userType.ToString());
}
