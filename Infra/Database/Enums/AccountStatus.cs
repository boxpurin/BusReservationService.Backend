using AccountStatusDomain = BusReservationService.Domain.ValueObject.AccountStatus;

namespace BusReservationService.Database.Enums;

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
    => Enum.Parse<AccountStatusDomain>(status.ToString());
}

