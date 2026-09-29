using BusReservationService.Database.Enums;
using BusReservationService.Domain.Entity;
using BusReservationService.Share;

namespace BusReservationService.Database.Table;

/// <summary>
/// 人そのもの
/// </summary>
public class UserTable : IConvertibleToDomain<UserEntity>
{
    public Guid AccountId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string StreetAddress { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? FaxNumber { get; set; }
    public UserType UserType { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public UserEntity ToDomain()
    {
        return UserEntity.CreateUserEntity(
            AccountId,
            FirstName,
            LastName,
            ZipCode,
            StreetAddress,
            PhoneNumber,
            FaxNumber,
            UserType.ToDomain()
        );
    }
}
