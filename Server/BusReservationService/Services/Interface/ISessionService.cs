namespace BusReservationService.Services.Interface;

public abstract class ISessionService<K, V> where K : notnull 
{
    protected IDictionary<K, V> SessionStorage { get; private set; } = new Dictionary<K, V>();

    public V? this[K key]
    {
        get { return SessionStorage[key]; }
        set { SessionStorage[key] = value; }
    }

    public abstract K CreateRandomKeySession(V value);

    public virtual bool TryGetSessionValue(K key, out V? value)
    {
        return SessionStorage.TryGetValue(key, out value);
    }

    public abstract void ClearSession(K key);
    public abstract void ClearAllSession();
}
