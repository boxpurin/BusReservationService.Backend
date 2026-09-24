namespace BusReservationService.Share;

public abstract record Result<T, E>
{
    public sealed record Ok(T Value) : Result<T, E>;
    public sealed record Err(E Error) : Result<T, E>;

    public bool IsOk() => GetType() == typeof(Ok);
    public bool IsError() => GetType() == typeof(Err);
}