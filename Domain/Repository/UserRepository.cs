using BusReservationService.Domain.Entity;
using BusReservationService.Domain.ValueObject.UserType;
using BusReservationService.Share;

namespace BusReservationService.Domain.Repository;

public interface IUserRepository
{
    Task<UserEntity> AddAsymc(AddAsymcDto dto);
    Task<UserEntity> UpdateAsync(UserEntity entity);
    Task<UserEntity?> FindByAccountIdAsync(Guid accountId);
    Task<bool> ExistByAccountIdAsync(Guid accountId);
    Task<bool> DeleteByAccountIdAsync(Guid accountId);
}

public record AddAsymcDto(
    Guid AccountId,
    string FirstName,
    string LastName,
    string ZipCode,
    string StreetAddress,
    string PhoneNumber,
    string? FaxNumber,
    UserType UserType
);