using BusReservationService.Database.Repository;
using BusReservationService.Domain.Entity;
using BusReservationService.Services.Interface;
using BusReservationService.Share;

namespace BusReservationService.Services;

public class UserService(UserRepository UserRepository) : IUserService
{
    public async Task<UserEntity?> OnBoardUserAsync(OnBoardUserDto dto)
    {
        var entity = new UserEntity(
                dto.AccountId, 
                dto.FirstName, 
                dto.LastName, 
                dto.PhoneNumber, 
                Domain.ValueObject.UserType.Personal
            );
        var result = await UserRepository.AddAsymc(entity);



        return entity;
    }
}
