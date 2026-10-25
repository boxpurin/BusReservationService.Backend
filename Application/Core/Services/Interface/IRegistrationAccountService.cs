using BusReservationService.Models;

namespace BusReservationService.Services.Interface;

public interface IRegistrationAccountService
{
    Task<AccountModel?> RegistrationAccountAsync(string email, string password);
}
