using BusReservationService.Domain.ValueObject.UserType;

namespace BusReservationService.Services.Interface;

public interface IOnboardUserProfileService
{
    Task OnboardUserProfile(Guid accountId, string firstName, string lastName, string zipCode, string streetAddress, string phoneNumber, string? faxNumber, UserType userType);
}