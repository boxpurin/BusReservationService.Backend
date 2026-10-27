using BusReservationService.Domain.Entity;
using BusReservationService.Domain.ValueObject.AccountRole;
using BusReservationService.Domain.ValueObject.AccountStatus;
using BusReservationService.Domain.ValueObject.UserType;

namespace BusReservationService.Models;

public record SessionValue(
    Guid AccountId, 
    string Email, 
    string FirstName, 
    string LastName,
    string PhoneNumber,
    AccountStatus Status,
    AccountRole Role,
    UserType UserType
)
{
    public SessionValue(AccountUserModel accountUserModel)
    : this(
        accountUserModel.AccountId, 
        accountUserModel.Email, 
        accountUserModel.FirstName, 
        accountUserModel.LastName,
        accountUserModel.PhoneNumber,
        accountUserModel.AccountStatus, 
        accountUserModel.AccountRole, 
        accountUserModel.UserType)
        {}

    public SessionValue(AccountEntity accountEntity, UserEntity userEntity)
    : this (
        accountEntity.Id,
        accountEntity.Email, 
        userEntity.FirstName,
        userEntity.LastName,
        userEntity.PhoneNumber,
        accountEntity.AccountStatus,
        accountEntity.AccountRole,
        userEntity.UserType)
    {}
}