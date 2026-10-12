using AccountStatusDomain = BusReservationService.Domain.ValueObject.AccountStatus;

namespace BusReservationService.Database.Enums;

internal enum AccountStatus
{
    JustCreated,
    PendingEmailVerify,
    OnBoarding,
    Activated,
    Suspended,
}

internal static class AccountStatusExtensions
{
    public static AccountStatusDomain ToDomain(this AccountStatus status) 
    => Enum.Parse<AccountStatusDomain>(status.ToString());
}

