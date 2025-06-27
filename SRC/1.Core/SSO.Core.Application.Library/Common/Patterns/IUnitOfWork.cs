using SSO.SharedKernel.Utilities.Library.Scrutor.Abstractions;

namespace SSO.Core.Application.Library.Common.Patterns;

public interface IUnitOfWork : IDisposable, IAsyncDisposable, IScopedLifetime
{
    //void BeginTransaction();
    //Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    //void CommitTransaction();
    //Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    //void RollbackTransaction();
    //Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    void SaveChanges();
    Task SaveChangesAsync(CancellationToken cancellationToken = default);


    Task TransactionAsync(Func<Task> func, CancellationToken cancellationToken = default);
    Task<TResult> TransactionAsync<TResult>(Func<Task<TResult>> func, CancellationToken cancellationToken = default);
}
