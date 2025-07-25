using SSO.Core.Domain.Library.Aggregates.Identity.RoleAggregate;

namespace SSO.Core.Application.Library.UseCases.Roles.Repoistories;

public interface IRoleQueryRepository : IQueryRepository<RoleEntity, long>
{
}
