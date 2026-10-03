using BusReservationService.Domain.Entity;
using BusReservationService.Share;

namespace BusReservationService.Domain.Repository;
public interface IReservationRepository
{
    /// <summary>
    /// 予約情報を追加する
    /// </summary>
    /// <param name="reservation">予約情報</param>
    /// <returns></returns>
    Task<ReservationEntity> AddAsync(ReservationEntity reservation);

    Task<ReservationEntity> UpdateAsync(ReservationEntity reservation);


    /// <summary>
    /// 予約IDから予約情報を取得する
    /// </summary>
    /// <param name="reservationId">予約ID</param>
    /// <returns></returns>
    Task<ReservationEntity?> FindByReservationIdAsync(Guid reservationId);

    /// <summary>
    /// 予約者のアカウントIDから予約情報一覧を取得する
    /// </summary>
    /// <param name="accountId">予約者のアカウントID</param>
    /// <returns></returns>
    Task<ReservationEntity[]> FindByAccountIdAsync(Guid accountId);

    /// <summary>
    /// 駐車場IDから予約情報一覧を取得する
    /// </summary>
    /// <param name="parkingLotId">駐車場ID</param>
    /// <returns></returns>
    Task<ReservationEntity[]> FindByParkingLotIdAsync(Guid parkingLotId);

    /// <summary>
    /// 予約IDが存在するかどうかを確認する
    /// </summary>
    /// <param name="reservationId">予約ID</param>
    /// <returns></returns>
    Task<bool?> ExistByReservationIdAsync(Guid reservationId);

    /// <summary>
    /// 予約者のアカウントIDが存在するかどうかを確認する
    /// </summary>
    /// <param name="accountId">予約者のアカウントID</param>
    /// <returns></returns>
    Task<bool> ExistByAccountIdAsync(Guid accountId);

    /// <summary>
    /// 予約IDから予約情報を削除する
    /// </summary>
    /// <param name="reservationId">予約ID</param>
    /// <returns></returns>
    Task<bool> DeleteByReservationIdAsync(Guid reservationId);

}
