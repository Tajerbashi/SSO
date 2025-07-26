using SSO.Core.Application.Library.Common.Models;
using SSO.Core.Domain.Library.Aggregates.Identity.UserAggregate;
using System.Security.Claims;

namespace SSO.Core.Application.Library.Models;

public class UserProfileDTO : BaseDTO
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Name { get; set; }
    public string Family { get; set; }
    public TokenResult TokenResult { get; set; }
    public List<Claim> Claims { get; set; }
    public UserEntity UserEntity { get; set; }

    public void BuildClaims(List<Claim> claims) => Claims = claims;
}

public class LoginSSOResult
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public string ReturnUrl { get; set; }
    public int ExpiresIn { get; set; }
    public string TokenType { get; set; }
    public string Scope { get; set; }   // Scopes for Server1 & Server2
}
public class LoginSSOParameter
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string ClientId { get; set; } // Optional (if using OAuth)
    public string GrantType { get; set; } // Or "authorization_code" for OAuth
    public string ReturnUrl { get; set; }
}
public class LoginParameter
{
    public string Username { get; set; }
    public string Password { get; set; }
    public bool IsRemember { get; set; }
    public string ReturnUrl { get; set; }
}

public class IdentityProcessParameters
{
    public UserEntity User { get; set; }
    public UserProfileDTO UserProfile { get; set; }
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpireTime { get; set; }
}

public class LogoutResult
{
    public string Message { get; set; }
    public bool IsSuccess { get; set; }
}

public class LoginResult
{
    public string Message { get; set; }
    public bool Succeeded { get; set; }

    public static LoginResult Success() => new() { Succeeded = true, Message = string.Empty };
    public static LoginResult Failed(string message) => new() { Succeeded = false, Message = message };
}

public class TokenParameter
{
    public string Username { get; set; }
}

public class TokenResult
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpireTime { get; set; }

    public TokenResult() { }

    public TokenResult(string refreshToken)
    {
        RefreshToken = refreshToken;
    }
}