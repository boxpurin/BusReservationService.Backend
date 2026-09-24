using AccountRoleDomain = BusReservationService.Domain.ValueObject.AccountRole;

namespace BusReservationService.Database.Enum;

public enum AccountRole
{
    Guest,
    User,
    Admin,
    LotAdmin,
}

public static class AccountRoleExtensions
{
    public static AccountRoleDomain ToDomain(this AccountRole role)
    {
        return role switch
        {
            AccountRole.Guest => AccountRoleDomain.Guest,
            AccountRole.User => AccountRoleDomain.User,
            AccountRole.LotAdmin => AccountRoleDomain.LotAdmin,
            AccountRole.Admin => AccountRoleDomain.Admin,
            _ => throw new ApplicationException()
        };
    }
    public static AccountRole FromDomain(this AccountRole role, AccountRoleDomain domain)
    {
        return domain switch
        {
            AccountRoleDomain.Guest => AccountRole.Guest,
            AccountRoleDomain.User => AccountRole.User,
            AccountRoleDomain.LotAdmin => AccountRole.LotAdmin,
            AccountRoleDomain.Admin => AccountRole.Admin,
            _ => throw new ApplicationException()
        };
    }
}