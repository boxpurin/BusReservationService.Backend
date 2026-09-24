namespace BusReservationService.Database.Table;

public class PrefecturesTable
{
    public short Id { get; set; }
    public required string Name { get; set; }
    public required string Kana { get; set; }
}