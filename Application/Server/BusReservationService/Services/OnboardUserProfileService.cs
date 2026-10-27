

using BusReservationService.Domain.Entity;
using BusReservationService.Domain.Repository;
using BusReservationService.Domain.ValueObject.UserType;
using BusReservationService.Models;
using BusReservationService.Services.Interface;

public class OnboarduserProfileService(IAccountRepository accountRepository, IUserRepository userRepository) 
: IOnboardUserProfileService
{
    public async Task<AccountModel?> OnboardUserProfile(Guid accountId, string firstName, string lastName, string zipCode, string streetAddress, string phoneNumber, string? faxNumber, UserType userType)
    {
        // アカウント情報を習得
        var account = await accountRepository.FindByAccountIdAsync(accountId);
        if (account is null)
        {
            return null;
        }

        // すでにユーザー情報が追加されているアカウントIDなのかを確認する
        if (await userRepository.ExistByAccountIdAsync(account.Id))
        {
            return null;
        }

        // ユーザーの登録
        var user = await userRepository.AddAsymc(new AddAsymcDto(
            accountId,
            firstName,
            lastName,
            zipCode,
            streetAddress,
            phoneNumber,
            faxNumber,
            userType
        ));

        // アカウントの有効化
        account.Activate();

        // アカウント情報の更新
        await accountRepository.UpdateAsync(account);

        return new AccountModel(account);
    }
}