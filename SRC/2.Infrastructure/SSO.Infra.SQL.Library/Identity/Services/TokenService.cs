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
    private readonly UserManager<UserIdentity> _userManager;
    private readonly RoleManager<RoleIdentity> _roleManager;
    private readonly SignInManager<UserIdentity> _signInManager;
    private readonly IConfiguration _configuration;

    private const string LoginProvider = "Token";
    private const string AuthKey = "AuthKey";
    private const string TokenName = "AccessToken";
    private const string RefreshToken = "RefreshToken";
    private const string ProviderDisplayName = "Auth Account";

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
    }

    public async Task<LoginResult> LoginAsync(LoginParameter parameter)
    {
        var user = await _userManager.FindByNameAsync(parameter.Username) ?? await _userManager.FindByEmailAsync(parameter.Username);
        if (user == null)
            return new LoginResult { Succeeded = false, Message = "Invalid credentials." };

        var result = await _signInManager.PasswordSignInAsync(user, parameter.Password, false, false);
        if (!result.Succeeded)
            return new LoginResult { Succeeded = false, Message = "Invalid credentials." };

        var tokenResult = await GenerateTokenAsync(new TokenParameter { User = UserIdentity.Map(user) });

        return new LoginResult
        {
            Succeeded = true,
            Message = string.Empty,
            User = UserIdentity.Map(user),
            Token = tokenResult.Token,
            RefreshToken = tokenResult.RefreshToken,
            ExpireTime = tokenResult.ExpireTime,
        };
    }

    public async Task<TokenResult> GenerateTokenAsync(TokenParameter parameter)
    {
        var profile = await GenerateProfileAsync(parameter.User.UserName);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Identity:Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Identity:Jwt:ExpireMinutes"] ?? "60"));

        // Generate secure refresh token (JWE encrypted)
        var refreshToken = GenerateSecureRefreshToken();
        var encryptedRefreshToken = EncryptRefreshToken(refreshToken);

        var token = new JwtSecurityToken(
            issuer: _configuration["Identity:Jwt:Issuer"],
            audience: _configuration["Identity:Jwt:Audience"],
            claims: profile.Claims,
            expires: expires,
            signingCredentials: creds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);



        await SaveRefreshTokenToUserAsync(parameter.User.Id.ToString(), encryptedRefreshToken);
        await AddUserLoginAsync(parameter.User.Id);
        await AddUserTokenAsync(parameter.User.Id, tokenString);

        return new TokenResult
        {
            Token = tokenString,
            ExpireTime = expires,
            RefreshToken = encryptedRefreshToken,
        };
    }

    private async Task<UserProfileDTO> GenerateProfileAsync(string username)
    {
        var user = await _userManager.FindByNameAsync(username)
                   ?? throw new DatabaseException($"User Not Found by username: {username}");

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName!),
            new("AuthKey", Guid.NewGuid().ToString()),
            new(ClaimTypes.Email, user.Email!)
        };

        var roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var profile = new UserProfileDTO();

        profile.BuildClaims(claims);

        var existingClaims = await _userManager.GetClaimsAsync(user);
        await _userManager.RemoveClaimsAsync(user, existingClaims);
        await _userManager.AddClaimsAsync(user, claims);

        return profile;
    }

    private string GenerateSecureRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    private string EncryptRefreshToken(string refreshToken)
    {
        var encryptionKey = _configuration["Identity:Jwe:Key"];
        var keyBytes = Encoding.UTF8.GetBytes(encryptionKey);
        using var aes = Aes.Create();
        aes.Key = keyBytes;
        aes.GenerateIV();

        var encryptor = aes.CreateEncryptor();
        var tokenBytes = Encoding.UTF8.GetBytes(refreshToken);
        var cipherBytes = encryptor.TransformFinalBlock(tokenBytes, 0, tokenBytes.Length);

        return Convert.ToBase64String(aes.IV.Concat(cipherBytes).ToArray());
    }

    private async Task SaveRefreshTokenToUserAsync(string userId, string encryptedToken)
    {
        var user = await _userManager.FindByIdAsync(userId)
                   ?? throw new DatabaseException("User not found");

        await _userManager.RemoveAuthenticationTokenAsync(user, LoginProvider, RefreshToken);

        var result = await _userManager.SetAuthenticationTokenAsync(user, LoginProvider, RefreshToken, encryptedToken);
        if (!result.Succeeded)
            throw new DatabaseException("Could not save refresh token.");
    }

    private async Task AddUserLoginAsync(long id)
    {
        var user = await _userManager.FindByIdAsync($"{id}");
        var result = await _userManager.RemoveLoginAsync(user, LoginProvider, user.EntityId.ToString());
        if (!result.Succeeded)
            throw new DatabaseException("Removing old login failed.");

        await _userManager.AddLoginAsync(user, new UserLoginInfo(LoginProvider, user.EntityId.ToString(), ProviderDisplayName));
    }

    private async Task AddUserTokenAsync(long id, string jwtToken)
    {
        var user = await _userManager.FindByIdAsync($"{id}");
        var result = await _userManager.SetAuthenticationTokenAsync(
            user,
            LoginProvider,
            TokenName,
            jwtToken
        );

        if (!result.Succeeded)
            throw new DatabaseException("Failed to update user token.");
    }

    public async Task<LogoutResult> LogoutAsync(string authKey)
    {
        await _signInManager.SignOutAsync();
        var user = (await _userManager.GetUsersForClaimAsync(new Claim(AuthKey, authKey)));
        if (user.Count > 0)
        {
            var entity = user.Single();
            
            //  Remove Claims
            var claims = await _userManager.GetClaimsAsync(entity);
            await _userManager.RemoveClaimsAsync(entity, claims);

            //  Remove Tokens
            await _userManager.RemoveAuthenticationTokenAsync(entity, LoginProvider, TokenName);
            await _userManager.RemoveAuthenticationTokenAsync(entity, LoginProvider, RefreshToken);

            // Remove Logins
            var loginEntity = await _userManager.GetLoginsAsync(entity);
            await _userManager.RemoveLoginAsync(entity,LoginProvider, loginEntity.Single().ProviderKey);
        }

        return new LogoutResult()
        {
            Message = authKey,
            IsSuccess = true
        };
    }
}

