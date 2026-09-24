using BusReservationService.Domain.Entity;
using BusReservationService.Share;

namespace BusReservationService.Services.Interface;

public interface IUserService
{
    Task<UserEntity?> OnBoardUserAsync(OnBoardUserDto dto);
}

public record OnBoardUserDto(Guid AccountId, string FirstName, string LastName, string PhoneNumber);