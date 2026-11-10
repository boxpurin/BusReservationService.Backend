using BusReservationService.Domain.ValueObject.UserType;
using BusReservationService.Models;
using BusReservationService.Share;
using BusReservationService.Share.Utils.Error;

namespace BusReservationService.Services.Interface;

public interface IOnboardUserProfileService
{
    Task<ApplicationResult<AccountModel, Error>> OnboardUserProfile(Guid accountId, string firstName, string lastName, string zipCode, string streetAddress, string phoneNumber, string? faxNumber, UserType userType);
}