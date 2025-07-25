using SSO.Core.Application.Library.UseCases.Users.Repoistories;
using SSO.Core.Domain.Library.Aggregates.Identity.UserAggregate;
using SSO.Infra.SQL.Library.Common.Repository;
using SSO.Infra.SQL.Library.Context;

namespace SSO.Infra.SQL.Library.UseCases.Users.Repositories;

public class UserQueryRepository : QueryRepository<UserEntity, long, DataContext>, IUserQueryRepository
{
    public UserQueryRepository(DataContext context) : base(context)
    {
    }
}
