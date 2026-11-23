using System.Data;
namespace BusReservationService;
public interface IUnitOfWork
{
    /// <summary>
    /// Begins a new unit of work transaction.
    /// </summary>
    /// <returns></returns>
    Task BeginAsync(IsolationLevel? isolationLevel);
    /// <summary>
    /// Rolls back the current unit of work transaction.
    /// </summary>
    /// <returns></returns>
    Task RollbackAsync();
    /// <summary>
    /// Commits the current unit of work transaction.
    /// </summary>
    /// <returns></returns>
    Task CommitAsync();
}