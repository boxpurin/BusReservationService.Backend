using BusReservationService.Domain.Entity;

namespace BusReservationService.Models;

public record SessionValue(Guid AccountId, string Email, string FirstName, string LastName){}