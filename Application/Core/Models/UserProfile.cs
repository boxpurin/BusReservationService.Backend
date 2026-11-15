using BusReservationService.Domain.Entity;
using BusReservationService.Domain.ValueObject.AccountRole;
using BusReservationService.Domain.ValueObject.AccountStatus;
using BusReservationService.Domain.ValueObject.UserType;
using BusReservationService.Models;

public class UserProfile
{
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public AccountRole Role { get; set; } = default!;
    public AccountStatus Status { get; set; } = default!;
    public UserType? UserType { get; set; }
    public UserProfile(){}
    
    public UserProfile(AccountEntity account, UserEntity user)
    {
        Email = account.Email;
        FirstName = user.FirstName;
        LastName = user.LastName;
        Role = account.AccountRole;
        Status = account.AccountStatus;
        UserType = user.UserType;
    }

    public UserProfile(AccountUserModel accountUserModel)
    {
        Email = accountUserModel.Email;
        FirstName = accountUserModel.FirstName;
        LastName = accountUserModel.LastName;
        Role = accountUserModel.AccountRole;
        Status = accountUserModel.AccountStatus;
        UserType = accountUserModel.UserType;
    }
}