using AccountStatusDomain = BusReservationService.Domain.ValueObject.AccountStatus.AccountStatus;

namespace BusReservationService.Database.Enums;

internal enum AccountStatus
{
    JustCreated = 0,
    PendingEmailVerify = 1,
    OnBoarding = 2,
    Activated = 3,
    Suspended = 4,
}

internal static class AccountStatusExtensions
{
    public static AccountStatusDomain ToDomain(this AccountStatus status) 
    => Enum.Parse<AccountStatusDomain>(status.ToString());
}

