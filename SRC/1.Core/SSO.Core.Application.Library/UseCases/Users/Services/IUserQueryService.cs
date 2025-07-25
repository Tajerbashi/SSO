using SSO.Core.Application.Library.Common.Service;
using SSO.Core.Domain.Library.Aggregates.Identity.UserAggregate;

namespace SSO.Core.Application.Library.UseCases.Users.Services;

public interface IUserQueryService : IQueryService<UserEntity, long>
{
}
