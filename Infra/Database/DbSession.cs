using System.Data;
using BusReservationService.Database.Interface;

namespace BusReservationService.Infra.Database;

public sealed class DbSession : IDbContext, IUnitOfWork, IDisposable
{
    private bool _disposed = false;
    public DbSession(IDbConnection connection)
    {
        Connection = connection;
        Transaction = null;
    }

    public Task BeginAsync(IsolationLevel? isolationLevel = null)
    {
        EnsureConnectionOpen();

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

        return Task.CompletedTask;
    }

    public Task RollbackAsync()
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

        return Task.CompletedTask;
    }

    public Task CommitAsync()
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

        return Task.CompletedTask;
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