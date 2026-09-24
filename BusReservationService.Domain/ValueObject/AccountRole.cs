namespace BusReservationService.Domain.ValueObject;

public enum AccountRole
{
    Guest = 0,
    User = 1,
    LotAdmin = 2,
    Admin = 3,
}

/// <summary>
/// 拡張メソッド
/// </summary>
public static class AccountRoleExtension
{
    // 権限を昇格する

    public static void GrantToLotAdmin(this AccountRole role)
    {
        role = AccountRole.LotAdmin;
    }

    public static void GrantToAdmin(this AccountRole role)
    {
        role = AccountRole.Admin;
    }

    // 権限を剥奪する
    public static void RevokeFromLotAdmin(this AccountRole role)
    {
        role = AccountRole.User;
    }

    public static void RevokeFromAdmin(this AccountRole role)
    {
        role = AccountRole.User;
    }

    public static bool IsLotAuthority(this AccountRole role) => role >= AccountRole.LotAdmin;
    public static bool IsSystemAuthority(this AccountRole role) => role is AccountRole.Admin;
}