using System.Data;
using BusReservationService.Database.Interface;

namespace BusReservationService.Infra.Database;

public sealed class DbSession : IConnection, IUnitOfWork, IDisposable
{
    private bool _disposed = false;
    public DbSession(IDbConnection connection)
    {
        Connection = connection;
    }

    public IDbConnection Connection { get; }
    public IDbTransaction? Transaction { get; private set; }

    public async Task BeginAsync(IsolationLevel? isolationLevel)
    {
        EnsureConnectionOpen(Connection);

        if (Transaction is null)
        {
            if (isolationLevel.HasValue)
            {
                Transaction = Connection.BeginTransaction(isolationLevel.Value);
            }
            else
            {
                Transaction = Connection.BeginTransaction();
            }
        }
    }

    public async Task RollbackAsync()
    {
        if (Transaction is not null)
        {
            try
            {
                Transaction.Rollback();
            }
            finally
            {
                Transaction.Dispose();
                Transaction = null;
                Connection.Close();
            }
        }
    }

    public async Task CommitAsync()
    {
        if (Transaction is null)
        {
            throw new InvalidOperationException("No active transaction to commit.");
        }
        try
        {
            Transaction.Commit();
        }
        finally
        {
            Transaction.Dispose();
            Transaction = null;
            Connection.Close();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                Transaction?.Dispose();
                Connection?.Dispose();
            }
            _disposed = true;
        }
    }
}