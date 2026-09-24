using BusReservationService.Domain.Entity;
using BusReservationService.Share;

namespace BusReservationService.Domain.Repository;
public interface IReservationRepository
{
    // 引数が多いのでこういうのはDtoにしてしまうべき気がする
    Task<ReservationEntity> AddAsync(ReservationEntity reservation);
    Task<ReservationEntity[]> AddListAsync(ReservationEntity[] reservations);

    Task<ReservationEntity> UpdateAsync(ReservationEntity reservation);
    Task<ReservationEntity[]> UpdateListAsync(ReservationEntity[] reservations);

    Task<ReservationEntity?> FindByReservationIdAsync(Guid reservationId);
    Task<ReservationEntity?> FindByAccountIdAsync(Guid accountId);

    Task<bool?> ExistByReservationIdAsync(Guid reservationId);
    Task<bool> ExistByAccountIdAsync(Guid accountId);

    // 予約内容そのものの削除、解約
    // 削除は個人情報保護の観点
    Task<bool> DeleteByReservationIdAsync(Guid reservationId);

}
