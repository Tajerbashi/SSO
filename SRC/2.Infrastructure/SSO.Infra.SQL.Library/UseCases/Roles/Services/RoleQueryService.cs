using SSO.Core.Application.Library.UseCases.Roles.Services;
using SSO.Core.Domain.Library.Aggregates.Identity.RoleAggregate;
using SSO.Infra.SQL.Library.Common.Repository;
using SSO.Infra.SQL.Library.Context;

namespace SSO.Infra.SQL.Library.UseCases.Roles.Services;

public class RoleQueryService: QueryRepository<RoleEntity, long, DataContext>, IRoleQueryService
{
    public RoleQueryService(DataContext context) : base(context)
    {
    }
}


