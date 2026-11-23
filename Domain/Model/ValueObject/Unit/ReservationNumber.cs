using UnitGenerator;

namespace BusReservationService.Domain.ValueObject.Unit;

[UnitOf<string>(
    options: UnitGenerateOptions.Normalize |
    UnitGenerateOptions.Validate |
    UnitGenerateOptions.JsonConverter |
    UnitGenerateOptions.DapperTypeHandler
)]
public partial struct ReservationNumberUnit {
    private partial void Normalize(ref string value) {
        value = value.Trim();
    }

    private partial void Validate()
    {
        if(value.Length != 6)
        {
            throw new ArgumentException("Reservation number must be 6 characters long.");
        }

        if(!value.All(char.IsDigit))
        {
            throw new ArgumentException("Reservation number must contain only digits.");
        }
    }
}