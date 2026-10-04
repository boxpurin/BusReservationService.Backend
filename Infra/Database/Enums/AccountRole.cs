using AccountRoleDomain = BusReservationService.Domain.ValueObject.AccountRole;

namespace BusReservationService.Database.Enums;

internal enum AccountRole
{
    Guest,
    User,
    Admin,
    LotAdmin,
}

internal static class AccountRoleExtensions
{
    public static AccountRoleDomain ToDomain(this AccountRole role) => 
    Enum.Parse<AccountRoleDomain>(role.ToString());
}