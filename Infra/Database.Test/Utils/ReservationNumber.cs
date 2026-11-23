using BusReservationService.Domain.ValueObject.Unit;
using System.Security.Cryptography;

namespace Infra.Database.Test.Utils;

internal static class ReservationNumberUtil
{
    public static ReservationNumberUnit Create(string value)
    {
        return new ReservationNumberUnit(value);
    }
    
    public static ReservationNumberUnit Create(int value)
    {
        return Create(value.ToString("D6"));
    }

    public static ReservationNumberUnit Random()
    {
        var value = RandomNumberGenerator.GetInt32(0, 1000000).ToString("D6");
        return Create(value);
    }
}