using SSO.Core.Application.Library.UseCases.Roles.Repoistories;
using SSO.Core.Domain.Library.Aggregates.Identity.RoleAggregate;
using SSO.Infra.SQL.Library.Common.Repository;
using SSO.Infra.SQL.Library.Context;

namespace SSO.Infra.SQL.Library.UseCases.Roles.Repositories;

public class RoleQueryRepository : QueryRepository<RoleEntity, long, DataContext>, IRoleQueryRepository
{
    public RoleQueryRepository(DataContext context) : base(context)
    {
    }
}


