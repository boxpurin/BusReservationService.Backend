namespace BusReservationService.Stores.Interface;

/// <summary>
/// アクティベーション用のセッションを管理するインターフェース
/// </summary>
public interface IActivationSessionStore
{
    /// <summary>
    /// 対応アカウントのセッションIDを発行しストアに保存する
    /// </summary>
    /// <param name="accountId">対象アカウントID</param>
    /// <param name="ct">キャンセルトークン</param>
    /// <returns>発行されたセッションID </returns>
    Task<string> CreateAsync(Guid accountId, CancellationToken ct);

    /// <summary>
    /// ストアに保存されたセッションIDに対応したアカウントIDを取得する
    /// 存在しない場合はGuid.Emptyを返す
    /// </summary>
    /// <param name="sessionId">セッションID</param>
    /// <param name="ct">キャンセルトークン</param>
    /// <returns>対応するアカウントID。存在しない場合はGuid.Empty</returns>
    Task<Guid> FindAccountIdAsync(string sessionId, CancellationToken ct);

    /// <summary>
    /// 対象のセッション情報を削除する
    /// </summary>
    /// <param name="sessionId">削除したいセッションID</param>
    /// <param name="ct">キャンセルトークン</param>
    /// <returns>削除処理が完了するまでのタスク</returns>
    Task DeleteAsync(string sessionId, CancellationToken ct);
}

/// <summary>
/// アクティベーション用セッションのオプション設定を表すクラス
/// デフォルトの有効期限では15分
/// </summary>
public sealed class ActivationSessionOptions
{
    public TimeSpan Lifetime { get; set; } = TimeSpan.FromMinutes(15);
}