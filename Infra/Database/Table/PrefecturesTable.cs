using BusReservationService.Domain.ValueObject.Prefecture;
using BusReservationService.Share;

namespace BusReservationService.Database.Table;

public class PrefecturesTable : IConvertibleToDomain<Prefecture>
{
    public short Id { get; set; }
    public required string Name { get; set; }
    public required string Kana { get; set; }

    public Prefecture ToDomain()
    {
        return PrefectureExtensions.FromIndex(Id);
    }
}