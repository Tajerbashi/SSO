using SSO.Core.Domain.Library.Aggregates.Identity.UserAggregate;

namespace SSO.Core.Application.Library.UseCases.Users.Repoistories;

public interface IUserCommandRepository : ICommandRepository<UserEntity, long>
{
}
