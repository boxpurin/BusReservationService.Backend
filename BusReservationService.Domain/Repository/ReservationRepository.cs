using BusReservationService.Domain.Entity;
using BusReservationService.Share;

namespace BusReservationService.Domain.Repository;
public interface IReservationRepository
{
    // 引数が多いのでこういうのはDtoにしてしまうべき気がする
    Task<ReservationEntity> AddAsync(ReservationEntity reservation);

    Task<ReservationEntity> UpdateAsync(ReservationEntity reservation);


    /// <summary>
    /// 予約IDから予約情報を取得する
    /// </summary>
    /// <param name="reservationId">予約ID</param>
    /// <returns></returns>
    Task<ReservationEntity?> FindByReservationIdAsync(Guid reservationId);
    Task<ReservationEntity?> FindByAccountIdAsync(Guid accountId);

    Task<bool?> ExistByReservationIdAsync(Guid reservationId);
    Task<bool> ExistByAccountIdAsync(Guid accountId);

    // 予約内容そのものの削除、解約
    // 削除は個人情報保護の観点
    Task<bool> DeleteByReservationIdAsync(Guid reservationId);

}
