using System;
using System.Collections.Generic;
using System.Text;

namespace BusReservationService.Domain.Entity;

public class PrefectureEntity
{
    public short Id { get; set; }
    public string Name { get; set; } = null!;
    public string Kana { get; set; } = null!;
}
