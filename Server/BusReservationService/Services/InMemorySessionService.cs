using BusReservationService.Models;
using BusReservationService.Services.Interface;

namespace BusReservationService.Services;

public class InMemorySessionService : ISessionService<Guid, SessionValue>
{
    public InMemorySessionService(){}

    public override void ClearAllSession()
    {
        SessionStorage.Clear();
    }

    public override void ClearSession(Guid key)
    {
        SessionStorage.Remove(key);
    }

    public override Guid CreateRandomKeySession(SessionValue value)
    {
        Guid key = Guid.CreateVersion7();

        SessionStorage.Add(key, value);
        Console.WriteLine(@"Session Stored. key {0} : value {1}", key, value);

        return key;
    }
}
