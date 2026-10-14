
using BusReservationService.Domain.Entity;

namespace BusReservationService.Database.Test.Utils;

internal static class TestUsers
{
    public static readonly UserEntity ExistPersonalUser = UserEntity.CreateUserEntity(
        Guid.Parse("00000000-0000-0000-0000-000000000001"),
        "FirstName", "LastName", 
        "1234567", "東京都千代田区1-1-1",
        "090-0000-0000", "06-0000-0000", Domain.ValueObject.UserType.Personal
    );

    public static readonly Guid NotExistId =
    Guid.Parse("99999999-0000-0000-0000-000000000001");
}