using BusReservationService.Models;
using BusReservationService.Share;
using BusReservationService.Share.Utils.Error;

namespace BusReservationService.Services.Interface;

public interface IAuthAccountService
{
    /// <summary>
    /// Sign in a user account with the specified email and password.
    /// </summary>
    /// <param name="email">The email of the account to sign in.</param>
    /// <param name="password">The password of the account.</param>
    /// <returns>The result of the sign-in operation.</returns>
    Task<ApplicationResult<AccountUserModel, Error>> SignInAccountAsync(string email, string password);
}