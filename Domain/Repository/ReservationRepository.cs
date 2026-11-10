using BusReservationService.Domain.Entity;
using BusReservationService.Share;

namespace BusReservationService.Domain.Repository;
public interface IReservationRepository
{
    /// <summary>
    /// 予約情報を追加する
    /// </summary>
    /// <param name="reservation">予約情報</param>
    /// <returns>追加された予約情報のエンティティ</returns>
    Task<ReservationEntity> AddAsync(ReservationEntity reservation);

    /// <summary>
    /// 予約
    /// </summary>
    /// <param name="reservation"></param>
    /// <returns>変更された予約情報のエンティティ</returns>
    Task<ReservationEntity> UpdateAsync(ReservationEntity reservation);

    /// <summary>
    /// 予約IDから予約情報を取得する
    /// </summary>
    /// <param name="reservationId">予約ID</param>
    /// <returns>対象の予約のエンティティ</returns>
    Task<ReservationEntity?> FindByReservationIdAsync(Guid reservationId);

    /// <summary>
    /// 予約者のアカウントIDから予約情報一覧を取得する
    /// </summary>
    /// <param name="accountId">予約者のアカウントID</param>
    /// <returns>対象アカウントの予約の一覧</returns>
    Task<ReservationEntity[]> FindByAccountIdAsync(Guid accountId);

    /// <summary>
    /// 駐車場IDから予約情報一覧を取得する
    /// </summary>
    /// <param name="parkingLotId">駐車場ID</param>
    /// <returns>対象駐車場の予約一覧</returns>
    Task<ReservationEntity[]> FindByParkingLotIdAsync(Guid parkingLotId);

    /// <summary>
    /// 駐車場IDの、指定した時間の間駐車予定の予約の一覧を取得する
    /// </summary>
    /// <param name="parkingLotId">駐車場ID</param>
    /// <param name="from">区間の開始時刻</param>
    /// <param name="to">区間の終了時刻</param>
    /// <returns>条件に一致する予約一覧</returns>
    Task<ReservationEntity[]> FindOverlappingByParkingLotIdAsync(Guid parkingLotId, DateTime from, DateTime to);

    /// <summary>
    /// 予約IDが存在するかどうかを確認する
    /// </summary>
    /// <param name="reservationId">予約ID</param>
    /// <returns>存在の有無</returns>
    Task<bool> ExistByReservationIdAsync(Guid reservationId);

    /// <summary>
    /// 予約IDから予約情報を削除する
    /// </summary>
    /// <param name="reservationId">予約ID</param>
    /// <returns>予約削除の成功の成否</returns>
    Task<bool> DeleteByReservationIdAsync(Guid reservationId);

    /// <summary>
    /// 予約IDから予約情報を削除する
    /// </summary>
    /// <param name="parkingLotId">駐車場ID</param>
    /// <returns>予約削除された件数</returns>
    Task<int> DeleteByParkingLotIdAsync(Guid parkingLotId);

    /// <summary>
    /// 予約IDから予約情報を削除する
    /// </summary>
    /// <param name="accountId">予約ID</param>
    /// <returns>予約削除された件数</returns>
    Task<int> DeleteByAccountIdAsync(Guid accountId);
}
