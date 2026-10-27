using BusReservationService.Models;
namespace BusReservationService.Services.Interface;

public interface IAccountUserQueryService
{
    Task<AccountUserModel?> GetModelByAccountIdAsync(Guid accountId);
    Task<AccountUserModel?> GetModelByEmailAsync(string email);
}