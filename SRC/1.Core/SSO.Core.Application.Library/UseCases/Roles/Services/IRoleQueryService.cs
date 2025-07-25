using SSO.Core.Domain.Library.Aggregates.Identity.RoleAggregate;

namespace SSO.Core.Application.Library.UseCases.Roles.Services;

public interface IRoleQueryService: IQueryService<RoleEntity, long>
{
}
