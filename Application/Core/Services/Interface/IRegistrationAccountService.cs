using BusReservationService.Models;
using BusReservationService.Share;
using BusReservationService.Share.Utils.Error;

namespace BusReservationService.Services.Interface;

public interface IRegistrationAccountService
{
    Task<ApplicationResult<AccountModel, Error>> RegistrationAccountAsync(string email, string password);
}
