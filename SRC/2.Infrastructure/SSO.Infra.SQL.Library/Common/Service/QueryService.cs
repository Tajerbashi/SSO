﻿using SSO.Core.Application.Library.Common.Service;

namespace SSO.Infra.SQL.Library.Common.Service;

public abstract class QueryService<TEntity, TId> : IQueryService<TEntity, TId>
    where TEntity : AggregateRoot<TId>
    where TId : struct,
          IComparable,
          IComparable<TId>,
          IConvertible,
          IEquatable<TId>,
          IFormattable
{
    protected readonly IQueryRepository<TEntity, TId> Repository;
    protected QueryService(IQueryRepository<TEntity, TId> repository)
    {
        Repository = repository;
    }
}