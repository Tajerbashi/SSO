using SSO.Core.Application.Library.Models;

namespace SSO.Core.Application.Library.Interfaces;

public interface ITokenService : IAutofacScopedLifetime
{
    Task<LoginResult> LoginAsync(LoginParameter parameter);
    Task<LogoutResult> LogoutAsync(string authKey);
    Task<TokenResult> GenerateTokenAsync(TokenParameter parameter);
}