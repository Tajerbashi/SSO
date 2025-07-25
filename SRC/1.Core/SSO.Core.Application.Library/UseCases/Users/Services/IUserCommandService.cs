using SSO.Core.Domain.Library.Aggregates.Identity.UserAggregate;

namespace SSO.Core.Application.Library.UseCases.Users.Services;

public interface IUserCommandService : ICommandService<UserEntity, long>
{
}
