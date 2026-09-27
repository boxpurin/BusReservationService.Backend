using AccountRoleDomain = BusReservationService.Domain.ValueObject.AccountRole;

namespace BusReservationService.Database.Enums;

public enum AccountRole
{
    Guest,
    User,
    Admin,
    LotAdmin,
}

public static class AccountRoleExtensions
{
    public static AccountRoleDomain ToDomain(this AccountRole role) => 
    Enum.Parse<AccountRoleDomain>(role.ToString());
}