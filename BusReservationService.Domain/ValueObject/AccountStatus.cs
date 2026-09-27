namespace BusReservationService.Domain.ValueObject;

public enum AccountStatus
{
    JustCreated = 0,
    PendingEmailVerify = 1,
    OnBoarding = 2,
    Active = 3,
    Suspended = 4,
}

public static class AccountStatusExteinsion
{
    public static bool IsJustCreated(this AccountStatus status) => status is AccountStatus.JustCreated;

    public static bool IsEmailVerifed(this AccountStatus status) => status > AccountStatus.PendingEmailVerify;

    public static bool IsOnBoarding(this AccountStatus status) => status is AccountStatus.OnBoarding;

    public static bool IsActivated(this AccountStatus status) => status is AccountStatus.Active;

    public static bool IsSuspend(this AccountStatus status) => status is AccountStatus.Suspended;

    public static void WaitVerifing(this AccountStatus status) => status = AccountStatus.PendingEmailVerify;

    public static void Verified(this AccountStatus status) => status = AccountStatus.OnBoarding;
    public static void Activate(this AccountStatus status) => status = AccountStatus.Active;
}