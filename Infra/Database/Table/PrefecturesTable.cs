using BusReservationService.Domain.Entity;
using BusReservationService.Share;

namespace BusReservationService.Database.Table;

public class PrefecturesTable : IConvertibleToDomain<PrefectureEntity>
{
    public short Id { get; set; }
    public required string Name { get; set; }
    public required string Kana { get; set; }

    public PrefectureEntity ToDomain()
    {
        throw new NotImplementedException();
    }
}