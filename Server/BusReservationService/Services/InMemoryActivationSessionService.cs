using BusReservationService.Models;
using BusReservationService.Services.Interface;

namespace BusReservationService.Services;

public class InMemoryActivationSessionService : ISessionService<Guid, ActivationSessionValue>
{
    public InMemoryActivationSessionService(){}

    public override void ClearAllSession()
    {
        SessionStorage.Clear();
    }

    public override void ClearSession(Guid key)
    {
        SessionStorage.Remove(key);
    }

    public override Guid CreateRandomKeySession(ActivationSessionValue value)
    {
        Guid key = Guid.CreateVersion7();

        // Ensure the generated key is unique
        while (SessionStorage.ContainsKey(key))
        {
            key = Guid.CreateVersion7();
        }

        SessionStorage.Add(key, value);
        return key;
    }
}
