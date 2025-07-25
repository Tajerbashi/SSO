using Microsoft.AspNetCore.Http.Features.Authentication;
using SSO.Core.Application.Library.UseCases.Roles.Repoistories;
using SSO.Core.Application.Library.UseCases.Users.Repoistories;

namespace SSO.Core.Application.Library.Interfaces;

public interface IIdentityService : IAutofacScopedLifetime
{
    IUserCommandRepository UserRepository { get; }
    IRoleCommandRepository RoleRepository { get; }
    ////IAuthenticationHandler AuthenticationHandler { get; }
    ////IHttpAuthenticationFeature HttpAuthenticationFeature { get; }
    ITokenService TokenService { get; }
}
