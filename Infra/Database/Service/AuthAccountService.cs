using BusReservationService.Services.Interface;
using BusReservationService.Models;
using BusReservationService.Share;
using BusReservationService.Share.Utils.Error;
using BusReservationService.Database.Interface;
using BusReservationService.Domain.Repository;
using BusReservationService.Util;
using System.Net;

namespace BusReservationService.Infra.Database.Service;

public class AuthUserAccountService(IAccountUserQueryService accountUserQueryService) : IConnection, IAuthAccountService
{

    public async Task<ApplicationResult<AccountUserModel, Error>> SignInAccountAsync(string email, string password)
    {
        var accountUser = await accountUserQueryService.GetModelByEmailAsync(email);
        if (accountUser is null)
        {
            return new Err<AccountUserModel, Error>(new Error{
                Message = "サインインに失敗しました。",
                ErrorCode = "SIGN_IN_FAILED",
                StatusCode = 401
            });
        }

        if(!Argon2Helper.VerifyPassword(password, accountUser.PasswordHash, accountUser.PasswordSalt))
        {
            return new Err<AccountUserModel, Error>(new Error{
                Message = "サインインに失敗しました。",
                ErrorCode = "SIGN_IN_FAILED",
                StatusCode = 401
            });
        }

        return new Ok<AccountUserModel, Error>(accountUser);
    }
}