using BusReservationService.Domain.ValueObject;
using System.ComponentModel.DataAnnotations;

namespace BusReservationService.Dto.Request;


public record RegistrationAccount(string Email, string Password, AccountRole Role);

/// <summary>
/// DTO for update account email request.
/// </summary>
/// <param name="NewEmail"></param>
public record UpdateAccountEmail(string NewEmail);