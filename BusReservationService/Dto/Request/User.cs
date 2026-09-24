namespace BusReservationService.Dto.Request;

/// <summary>
/// DTO for creating a new account profile
/// </summary>
/// <param name="FirstName"></param>
/// <param name="LastName"></param>
/// <param name="Phone"></param>
public record CreateUserProfile(string FirstName, string LastName, string Phone);

/// <summary>
/// DTO for updating an existing account profile
/// </summary>
/// <param name="FirstName"></param>
/// <param name="LastName"></param>
/// <param name="Phone"></param>
public record UpdateUserProfile(string FirstName, string LastName, string Phone);