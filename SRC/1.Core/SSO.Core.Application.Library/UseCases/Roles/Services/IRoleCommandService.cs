using SSO.Core.Domain.Library.Aggregates.Identity.RoleAggregate;

namespace SSO.Core.Application.Library.UseCases.Roles.Services;

public interface IRoleCommandService: ICommandService<RoleEntity, long>
{
}
