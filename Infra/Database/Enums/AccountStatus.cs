using AccountStatusDomain = BusReservationService.Domain.ValueObject.AccountStatus;

namespace BusReservationService.Database.Enums;

internal enum AccountStatus
{
    JustCreated,
    PendingEmailVerify,
    OnBoarding,
    Active,
    Suspended,
}

internal static class AccountStatusExtensions
{
    public static AccountStatusDomain ToDomain(this AccountStatus status) 
    => Enum.Parse<AccountStatusDomain>(status.ToString());
}

