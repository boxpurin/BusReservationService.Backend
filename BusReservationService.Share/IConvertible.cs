namespace BusReservationService.Share;

/// <summary>
/// なんらかのドメインの型Tへの変換を可能とする
/// </summary>
/// <typeparam name="T">変換先の型</typeparam>
public interface IConvertibleToDomain<T>
{
    public T ToDomain();
}