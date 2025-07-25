using SSO.Core.Application.Library.UseCases.Roles.Repoistories;
using SSO.Core.Application.Library.UseCases.Users.Repoistories;

namespace SSO.Infra.SQL.Library.Identity.Services;

public class IdentityService : IIdentityService
{
    private readonly IUserCommandRepository _userCommandRepository;
    private readonly IRoleCommandRepository _roleCommandRepository;
    private readonly ITokenService _tokenService;
    public IdentityService(
        IUserCommandRepository userCommandRepository,
        IRoleCommandRepository roleCommandRepository,
        ITokenService tokenService)
    {
        _userCommandRepository = userCommandRepository;
        _roleCommandRepository = roleCommandRepository;
        _tokenService = tokenService;
    }
    public IUserCommandRepository UserRepository => _userCommandRepository;
    public IRoleCommandRepository RoleRepository => _roleCommandRepository;
    public ITokenService TokenService => _tokenService;

}

