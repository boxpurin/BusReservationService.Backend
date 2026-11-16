using BusReservationService.Share;
using BusReservationService.Share.Utils.Error;

public interface IParkingLotCommandService
{
    /// <summary>
    /// 対象の駐車場の指定の日時に予約可能なスペースの数を取得する
    /// </summary>
    /// <param name="parkingLotId">対象駐車場ID</param>
    /// <param name="datetime">対象日時</param>
    /// <returns>指定の日時に予約可能なスペースの数</returns>
    Task<ApplicationResult<int, Error>> CountAvailableReservationSpace(Guid parkingLotId, DateTime datetime);

    /// <summary>
    /// 対象の駐車場の指定の日時に予約可能なスペースがあるかを確認する
    /// </summary>
    /// <param name="parkingLotId">対象駐車場ID</param>
    /// <param name="datetime">対象日時</param>
    /// <returns>指定の日時に予約可能なスペースがあるかどうか</returns>
    Task<ApplicationResult<bool, Error>> HasAvailableReservationSpace(Guid parkingLotId, DateTime datetime);
}