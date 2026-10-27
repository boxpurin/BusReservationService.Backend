using BusReservationService.Domain.ValueObject.UserType;
using BusReservationService.Models;

namespace BusReservationService.Services.Interface;

public interface IOnboardUserProfileService
{
    Task<AccountModel?> OnboardUserProfile(Guid accountId, string firstName, string lastName, string zipCode, string streetAddress, string phoneNumber, string? faxNumber, UserType userType);
}