using BusReservationService.Share;
using BusReservationService.Share.Utils.Error;

namespace Application.Core.Services.Interface.Administrator;

public interface IAdministratorReservationCommandService
{
    /// <summary>
    /// 予約を削除する
    /// </summary>
    /// <param name="adminAccountId">操作を行う管理者のアカウントID</param>
    /// <param name="reservationId">削除する予約のID</param>
    /// <returns>削除結果</returns>
    Task<ApplicationResult<int, Error>> DeleteReservationAsync(Guid adminAccountId, Guid reservationId);
}