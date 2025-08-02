using Microsoft.IdentityModel.Tokens;
using SSO.Core.Application.Library.Interfaces;
using SSO.Core.Application.Library.Models;
using SSO.Core.Domain.Library.Aggregates.Identity.UserAggregate;
using SSO.Infra.SQL.Library.Common.Exceptions;
using SSO.Infra.SQL.Library.Extensions;
using SSO.Infra.SQL.Library.Identity.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace SSO.Infra.SQL.Library.Identity.Services;

public class TokenService : ITokenService
{
    private const string LoginProvider = "Token";
    private const string AuthKey = "AuthKey";
    private const string TokenName = "AccessToken";
    private const string RefreshTokenName = "RefreshToken";
    private const string ProviderDisplayName = "Auth Account";

    private readonly UserManager<UserIdentity> _userManager;
    private readonly RoleManager<RoleIdentity> _roleManager;
    private readonly SignInManager<UserIdentity> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly JwtSecurityTokenHandler _tokenHandler;

    public TokenService(
        UserManager<UserIdentity> userManager,
        RoleManager<RoleIdentity> roleManager,
        SignInManager<UserIdentity> signInManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _tokenHandler = new JwtSecurityTokenHandler();
    }

    public async Task<LoginResult> LoginAsync(LoginParameter parameter)
    {
        var user = await FindUserByIdentifierAsync(parameter.Username);
        if (user == null)
            return LoginResult.Failed("Invalid credentials.");

        var result = await _signInManager.PasswordSignInAsync(user, parameter.Password, parameter.IsRemember, lockoutOnFailure: true);
        if (!result.Succeeded)
            return LoginResult.Failed("Invalid credentials.");

        var role = await _userManager.GetRolesAsync(user);

        // Create claims identity with required claims
        var claims = new List<Claim>
        {
            new Claim("sub", $"{user.Id}"),
            new Claim("name", user.UserName),
            new Claim("role", role.First()),
            new Claim("UserId", $"{user.Id}"),
            new Claim("UserName", user.UserName),
            new Claim("Name", user.FirstName),
            new Claim("Family", user.LastName),
            new Claim("Email", user.Email),
            // Add any additional claims your application needs
        };
        var identity = new ClaimsIdentity(claims, "Identity.Application");
        var principal = new ClaimsPrincipal(identity);

        // Sign in with the properly constructed principal
        await _signInManager.SignInWithClaimsAsync(
            user,
            parameter.IsRemember,
            claims);

        return LoginResult.Success();
    }

    public async Task<TokenResult> GenerateTokenAsync(TokenParameter parameter)
    {
        var user = await FindUserByIdentifierAsync(parameter.Username)
            ?? throw new DatabaseException($"User not found: {parameter.Username}");

        var profile = await GenerateUserProfileAsync(user);
        var token = GenerateJwtToken(profile.Claims);
        var tokenString = _tokenHandler.WriteToken(token);

        await UpdateUserTokensAsync(user, tokenString, profile.TokenResult.RefreshToken);

        return new TokenResult
        {
            Token = tokenString,
            ExpireTime = token.ValidTo,
            RefreshToken = profile.TokenResult.RefreshToken
        };
    }

    public async Task<LogoutResult> LogoutAsync(string authKey)
    {
        await _signInManager.SignOutAsync();

        var users = await _userManager.GetUsersForClaimAsync(new Claim(AuthKey, authKey));
        if (users.Count > 0)
        {
            var user = users.Single();
            await CleanUpUserTokensAndClaimsAsync(user);
        }

        return new LogoutResult
        {
            Message = authKey,
            IsSuccess = true
        };
    }

    private async Task<UserIdentity> FindUserByIdentifierAsync(string identifier)
    {
        return await _userManager.FindByNameAsync(identifier)
            ?? await _userManager.FindByEmailAsync(identifier);
    }

    private async Task<UserProfileDTO> GenerateUserProfileAsync(UserIdentity user)
    {
        var refreshToken = GenerateSecureRefreshToken();
        var claims = await BuildUserClaimsAsync(user, refreshToken);

        var profile = new UserProfileDTO();
        profile.BuildClaims(claims);
        profile.TokenResult = new TokenResult(refreshToken);

        await UpdateUserClaimsAsync(user, claims);

        return profile;
    }

    private async Task<List<Claim>> BuildUserClaimsAsync(UserIdentity user, string refreshToken)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName!),
            new(AuthKey, Guid.NewGuid().ToString()),
            new(ClaimTypes.Email, user.Email!),
            new(RefreshTokenName, refreshToken)
        };

        var roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        return claims;
    }

    private async Task UpdateUserClaimsAsync(UserIdentity user, List<Claim> claims)
    {
        var existingClaims = await _userManager.GetClaimsAsync(user);
        await _userManager.RemoveClaimsAsync(user, existingClaims);
        await _userManager.AddClaimsAsync(user, claims);
    }

    private JwtSecurityToken GenerateJwtToken(IEnumerable<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration["Identity:Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured")));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddMinutes(
            double.Parse(_configuration["Identity:Jwt:ExpireMinutes"] ?? "60"));

        return new JwtSecurityToken(
            issuer: _configuration["Identity:Jwt:Issuer"],
            audience: _configuration["Identity:Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds);
    }

    private string GenerateSecureRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        var encryptionKey = _configuration["Identity:Jwe:Key"]
            ?? throw new InvalidOperationException("JWE Key not configured");

        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(encryptionKey);
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        var cipherBytes = encryptor.TransformFinalBlock(randomBytes, 0, randomBytes.Length);

        return Convert.ToBase64String(aes.IV.Concat(cipherBytes).ToArray());
    }

    private async Task UpdateUserTokensAsync(UserIdentity user, string accessToken, string refreshToken)
    {
        await SaveRefreshTokenAsync(user, refreshToken);
        await UpdateUserLoginAsync(user);
        await SaveAccessTokenAsync(user, accessToken);
    }

    private async Task SaveRefreshTokenAsync(UserIdentity user, string refreshToken)
    {
        await _userManager.RemoveAuthenticationTokenAsync(user, LoginProvider, RefreshTokenName);

        var result = await _userManager.SetAuthenticationTokenAsync(
            user, LoginProvider, RefreshTokenName, refreshToken);

        if (!result.Succeeded)
            throw new DatabaseException("Failed to save refresh token.");
    }

    private async Task SaveAccessTokenAsync(UserIdentity user, string accessToken)
    {
        var result = await _userManager.SetAuthenticationTokenAsync(
            user, LoginProvider, TokenName, accessToken);

        if (!result.Succeeded)
            throw new DatabaseException("Failed to save access token.");
    }

    private async Task UpdateUserLoginAsync(UserIdentity user)
    {
        var result = await _userManager.RemoveLoginAsync(user, LoginProvider, user.EntityId.ToString());
        if (!result.Succeeded)
            throw new DatabaseException("Failed to remove old login.");

        await _userManager.AddLoginAsync(user,
            new UserLoginInfo(LoginProvider, user.EntityId.ToString(), ProviderDisplayName));
    }

    private async Task CleanUpUserTokensAndClaimsAsync(UserIdentity user)
    {
        // Remove Claims
        var claims = await _userManager.GetClaimsAsync(user);
        await _userManager.RemoveClaimsAsync(user, claims);

        // Remove Tokens
        await _userManager.RemoveAuthenticationTokenAsync(user, LoginProvider, TokenName);
        await _userManager.RemoveAuthenticationTokenAsync(user, LoginProvider, RefreshTokenName);

        // Remove Logins
        var logins = await _userManager.GetLoginsAsync(user);
        await _userManager.RemoveLoginAsync(user, LoginProvider, logins.Single().ProviderKey);
    }
}
