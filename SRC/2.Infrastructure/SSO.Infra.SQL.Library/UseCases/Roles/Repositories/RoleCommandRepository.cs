using SSO.Core.Application.Library.UseCases.Roles.Repoistories;
using SSO.Core.Domain.Library.Aggregates.Identity.RoleAggregate;
using SSO.Infra.SQL.Library.Common.Repository;
using SSO.Infra.SQL.Library.Context;

namespace SSO.Infra.SQL.Library.UseCases.Roles.Repositories;

public class RoleCommandRepository : CommandRepository<RoleEntity, long, DataContext>, IRoleCommandRepository
{
    public RoleCommandRepository(DataContext context) : base(context)
    {
    }
}


