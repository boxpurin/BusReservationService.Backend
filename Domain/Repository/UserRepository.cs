using BusReservationService.Domain.Entity;
using BusReservationService.Domain.ValueObject;
using BusReservationService.Share;

namespace BusReservationService.Domain.Repository;

public interface IUserRepository
{
    Task<UserEntity> AddAsymc(AddAsymcDto dto);
    Task<UserEntity> UpdateAsync(UserEntity entity);
    Task<UserEntity?> FindByAccountId(Guid accountId);
    Task<bool> ExistByAccountId(Guid accountId);
    Task<bool> DeleteById(Guid accountId);
}

public record AddAsymcDto(
    Guid AccountId,
    string FirstName,
    string LastName,
    string ZipCode,
    string StreetAdress,
    string PhoneNumber,
    string? FaxNumber,
    UserType UserType
);