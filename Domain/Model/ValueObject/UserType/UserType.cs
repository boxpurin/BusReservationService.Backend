namespace BusReservationService.Domain.ValueObject.UserType;
public enum UserType
{
    // 個人顧客
    Personal = 1,
    // 法人顧客
    Business = 2,
    // 管理者
    Administrator = 3,
}

public static class UserTypeExtension
{
    public static bool IsAdmin(this UserType type) => type == UserType.Administrator;

    public static bool IsPersonal(this UserType type) => type == UserType.Personal;
}