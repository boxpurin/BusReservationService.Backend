using BusReservationService.Models;

public interface IAccountUserQueryService
{
    Task<AccountUserModel?> GetModelByAccountIdAsync(Guid accountId);
    Task<AccountUserModel?> GetModelByEmailAsync(string email);
}