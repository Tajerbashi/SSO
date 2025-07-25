using SSO.Core.Application.Library.UseCases.Users.Repoistories;
using SSO.Core.Domain.Library.Aggregates.Identity.UserAggregate;
using SSO.Infra.SQL.Library.Common.Repository;
using SSO.Infra.SQL.Library.Context;

namespace SSO.Infra.SQL.Library.UseCases.Users.Repositories;

public class UserCommandRepository : CommandRepository<UserEntity, long, DataContext>, IUserCommandRepository
{
    public UserCommandRepository(DataContext context) : base(context)
    {
    }
}
