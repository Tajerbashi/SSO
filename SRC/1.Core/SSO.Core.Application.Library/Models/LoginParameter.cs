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
    public List<Claim> Claims { get; set; }
    public void BuildClaims(List<Claim> claims) => Claims = claims;

    public UserEntity UserEntity { get; set; }
}

public class LoginParameter
{
    public string Username { get; set; }
    public string Password { get; set; }
    public bool IsRemember { get; set; }
    public string ReturnUrl { get; set; }

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
    public UserEntity User { get; set; }

    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpireTime { get; set; }
    public UserProfileDTO UserProfile { get; set; }

}
public class TokenParameter
{
    public UserEntity User { get; set; }
}
public class TokenResult
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpireTime { get; set; }
}
