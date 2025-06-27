using SSO.Core.Application.Library.Common.Service;

namespace SSO.Infra.SQL.Library.Common.Service;

public abstract class CommandService<TEntity, TId> : ICommandService<TEntity, TId>
    where TEntity : AggregateRoot<TId>
    where TId : struct,
          IComparable,
          IComparable<TId>,
          IConvertible,
          IEquatable<TId>,
          IFormattable
{
    protected readonly ICommandRepository<TEntity, TId> Repository;
    protected CommandService(ICommandRepository<TEntity, TId> repository)
    {
        Repository = repository;
    }
}
