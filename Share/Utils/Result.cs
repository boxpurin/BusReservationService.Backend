namespace BusReservationService.Share;

public abstract record ApplicationResult<T, E>
{
    public bool IsOk() => GetType() == typeof(Ok<T, E>);
    public bool IsError() => GetType() == typeof(Err<T, E>);

    public T Unwrap()
    {
        if (IsOk())
        {
            return ((Ok<T, E>)this).Value;
        }
        throw new InvalidOperationException("Cannot unwrap a failure result.");
    }
    public E UnwrapError()
    {
        if (IsError())
        {
            return ((Err<T, E>)this).Value;
        }
        throw new InvalidOperationException("Cannot unwrap a success result.");
    }
}


public sealed record Ok<T, E>(T Value) : ApplicationResult<T, E>;
public sealed record Err<T, E>(E Value) : ApplicationResult<T, E>;