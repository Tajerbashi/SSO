using SSO.Core.Application.Library.UseCases.Users.Repoistories;
using SSO.Core.Application.Library.UseCases.Users.Services;
using SSO.Core.Domain.Library.Aggregates.Identity.UserAggregate;
using SSO.Infra.SQL.Library.Common.Service;

namespace SSO.Infra.SQL.Library.UseCases.Users.Services;

public class UserCommandService : CommandService<UserEntity, long>, IUserCommandService
{
    public UserCommandService(IUserCommandRepository repository) : base(repository)
    {
    }
}
