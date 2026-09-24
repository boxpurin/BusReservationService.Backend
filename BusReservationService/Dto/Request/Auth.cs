namespace BusReservationService.Dto.Request;

/// <summary>
/// DTO for account signup request
/// </summary>
/// <param name="Email"></param>
/// <param name="Password"></param>
public record SignupAccount(string Email, string Password);

/// <summary>
/// DTO for account signin request
/// </summary>
/// <param name="Email"></param>
/// <param name="Password"></param>
/// <param name="RememberMe"></param>
public record SigninAccount(string Email, string Password, bool RememberMe);

/// <summary>
/// DTO for account forgot password request
/// </summary>
/// <param name="Email"></param>
/// <param name="NewPassword"></param>
/// <param name="Token"></param>
public record UpdateForgotPassword(string Email, string NewPassword, string Token);