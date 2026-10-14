using Org.BouncyCastle.Crypto;

namespace BusReservationService.Database.Test.Utils;

internal static class TestAccountIds
{
    public static readonly Guid[] IDList = [
        Guid.Parse("00000000-0000-0000-0000-000000000001"),        
        Guid.Parse("00000000-0000-0000-0000-000000000002"),       
        Guid.Parse("00000000-0000-0000-0000-000000000003"),       
        Guid.Parse("00000000-0000-0000-0000-000000000004"),       
        Guid.Parse("00000000-0000-0000-0000-000000000005"),       
        Guid.Parse("00000000-0000-0000-0000-000000000006"),      
        Guid.Parse("00000000-0000-0000-0000-000000000007"),        
        Guid.Parse("00000000-0000-0000-0000-000000000008"),       
        Guid.Parse("00000000-0000-0000-0000-000000000009"),        
        Guid.Parse("00000000-0000-0000-0000-000000000010")
    ];
    public static readonly Guid NotExistId =
        Guid.Parse("99999999-0000-0000-0000-000000000001");
}