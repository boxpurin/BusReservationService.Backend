using AccountStatusDomain = BusReservationService.Domain.ValueObject.AccountStatus;

namespace BusReservationService.Database.Enum;

public enum AccountStatus
{
    JustCreated,
    PendingEmailVerify,
    OnBoarding,
    Active,
    Suspended,
}

public static class AccountStatusExtensions
{
    public static AccountStatusDomain ToDomain(this AccountStatus status)
    {
        return status switch
        {
            AccountStatus.JustCreated => AccountStatusDomain.JustCreated,
            AccountStatus.PendingEmailVerify => AccountStatusDomain.PendingEmailVerify,
            AccountStatus.OnBoarding => AccountStatusDomain.OnBoarding,
            AccountStatus.Active => AccountStatusDomain.Active,
            AccountStatus.Suspended => AccountStatusDomain.Suspended,
            _ => throw new ApplicationException("Invalid AccountStatus. Cast To Domain.")
        };
    }

    public static AccountStatus FromDomain(this AccountStatus status, AccountStatusDomain domain)
    {
        return domain switch
        {
            AccountStatusDomain.JustCreated => AccountStatus.JustCreated,
            AccountStatusDomain.PendingEmailVerify => AccountStatus.PendingEmailVerify,
            AccountStatusDomain.OnBoarding => AccountStatus.OnBoarding,
            AccountStatusDomain.Active => AccountStatus.Active,
            AccountStatusDomain.Suspended => AccountStatus.Suspended,
            _ => throw new ApplicationException("Invalid AccountStatus. Cast From Domain")
        };
    }
}

