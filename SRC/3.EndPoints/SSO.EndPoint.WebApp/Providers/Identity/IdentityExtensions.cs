using Duende.IdentityModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SSO.EndPoint.WebApp.Providers.Identity.Handlers;
using SSO.EndPoint.WebApp.Providers.Identity.Options;
using SSO.Infra.SQL.Library.Common.Constants;
using System.Text.Json;

namespace SSO.EndPoint.WebApp.Providers.Identity;

public static class IdentityExtensions
{
    public static IServiceCollection AddIdentityConfiguration(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName)
    {
        services.Configure<IdentityOption>(configuration.GetSection(sectionName));

        services.AddIdentityCoreServices(configuration)
                .AddAuthenticationStrategies(configuration, sectionName)
                .AddAuthorizationPolicies();

        return services;
    }

    private static IServiceCollection AddIdentityCoreServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddDistributedMemoryCache();
        services.AddSession(ConfigureSessionOptions);

        services.AddIdentity<UserIdentity, RoleIdentity>(ConfigureIdentityOptions)
            .AddEntityFrameworkStores<DataContext>()
            .AddRoles<RoleIdentity>()
            .AddDefaultTokenProviders()
            .AddApiEndpoints();

        return services;
    }

    private static void ConfigureSessionOptions(SessionOptions options)
    {
        options.IdleTimeout = TimeSpan.FromMinutes(30);
        options.Cookie.Name = "_auth.Session";
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    }

    private static void ConfigureIdentityOptions(IdentityOptions options)
    {
        options.ClaimsIdentity.UserIdClaimType = JwtClaimTypes.Subject;
        options.ClaimsIdentity.UserNameClaimType = JwtClaimTypes.Name;
        options.ClaimsIdentity.RoleClaimType = JwtClaimTypes.Role;

        // Password settings
        options.Password.RequiredLength = 6;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false;

        // Lockout settings
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;

        // User settings
        options.User.RequireUniqueEmail = true;
    }

    private static IServiceCollection AddAuthenticationStrategies(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName)
    {
        var identityOption = configuration.GetSection(sectionName).Get<IdentityOption>();

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = "JWT_OR_COOKIE";
            options.DefaultChallengeScheme = "JWT_OR_COOKIE";
        })
        .AddPolicyScheme("JWT_OR_COOKIE", "JWT_OR_COOKIE", options =>
        {
            options.ForwardDefaultSelector = context =>
                GetAuthenticationSchemeFromRequest(context.Request);
        })
        .AddCookie("AuthorizationCookies", options => ConfigureCookieAuthentication(options, identityOption))
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => ConfigureJwtBearerOptions(options, identityOption));

        return services;
    }

    private static string GetAuthenticationSchemeFromRequest(HttpRequest request)
    {
        var authorization = request.Headers.Authorization.FirstOrDefault();

        if (!string.IsNullOrEmpty(authorization) &&
            authorization.StartsWith("Bearer ") &&
            IsValidJwtStructure(authorization["Bearer ".Length..].Trim()))
        {
            return JwtBearerDefaults.AuthenticationScheme;
        }

        return "AuthorizationCookies";
    }

    private static bool IsValidJwtStructure(string token)
    {
        return token.Count(c => c == '.') == 2; // Basic JWT structure check
    }

    private static void ConfigureCookieAuthentication(
        CookieAuthenticationOptions options,
        IdentityOption identityOption)
    {
        options.Cookie.Name = "_auth.TK";
        options.LoginPath = "/Account/Login";
        options.ReturnUrlParameter = "ReturnUrl";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(identityOption.Jwt.ExpireMinutes);

        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = context =>
            {
                if (IsApiRequest(context.Request))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                }
                // For MVC/Razor Pages
                context.Response.Redirect(
                    $"{options.LoginPath}?{options.ReturnUrlParameter}={context.Request.Path}");
                return Task.CompletedTask;
            },

            OnRedirectToAccessDenied = context =>
            {
                if (IsApiRequest(context.Request))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                }

                context.Response.Redirect(options.AccessDeniedPath);
                return Task.CompletedTask;
            }
        };
    }

    private static bool IsApiRequest(HttpRequest request)
    {
        return request.Path.StartsWithSegments("/api") ||
               request.Headers["Accept"].ToString().Contains("application/json");
    }

    private static void ConfigureJwtBearerOptions(
        JwtBearerOptions options,
        IdentityOption identityOption)
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = identityOption.Jwt.Issuer,
            ValidAudience = identityOption.Jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(identityOption.Jwt.Key)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };

        options.Events = CreateJwtBearerEvents();
    }

    private static JwtBearerEvents CreateJwtBearerEvents()
    {
        return new JwtBearerEvents
        {
            OnMessageReceived = OnTokenReceived,
            OnAuthenticationFailed = OnAuthenticationFailure,
            OnTokenValidated = OnTokenValidation,
            OnChallenge = OnChallengeResponse
        };
    }

    private static Task OnTokenReceived(MessageReceivedContext context)
    {
        var authorization = context.Request.Headers["Authorization"].FirstOrDefault();

        if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
        {
            context.Token = authorization["Bearer ".Length..].Trim();

            if (string.IsNullOrEmpty(context.Token) || !IsValidJwtStructure(context.Token))
            {
                context.Fail("Invalid token format");
            }
        }
        else
        {
            context.Token = context.Request.Cookies["access_token"];
        }

        return Task.CompletedTask;
    }

    private static Task OnAuthenticationFailure(AuthenticationFailedContext context)
    {
        if (context.Exception is SecurityTokenExpiredException)
        {
            context.Response.Headers.Append("Token-Expired", "true");
        }
        Console.WriteLine($"Authentication failed: {context.Exception}");
        return Task.CompletedTask;
    }

    private static Task OnTokenValidation(TokenValidatedContext context)
    {
        Console.WriteLine("Token successfully validated");
        return Task.CompletedTask;
    }

    private static Task OnChallengeResponse(JwtBearerChallengeContext context)
    {
        context.HandleResponse();
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";

        var errorMessage = context.AuthenticateFailure switch
        {
            SecurityTokenExpiredException => "Token expired",
            SecurityTokenInvalidAudienceException => "Invalid audience",
            SecurityTokenInvalidIssuerException => "Invalid issuer",
            _ => "Unauthorized"
        };

        var result = JsonSerializer.Serialize(new { error = errorMessage });
        return context.Response.WriteAsync(result);
    }

    private static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            ConfigureDefaultPolicies(options);
            ConfigureRoleBasedPolicies(options);
            options.AddPolicy("Over18", policy =>
                policy.Requirements.Add(new MinimumAgeRequirement(18)));
        });

        return services;
    }

    private static void ConfigureDefaultPolicies(AuthorizationOptions options)
    {
        options.DefaultPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddAuthenticationSchemes("JWT_OR_COOKIE")
            .Build();
    }

    private static void ConfigureRoleBasedPolicies(AuthorizationOptions options)
    {
        // Admin policies
        options.AddPolicy(Policies.CanPurge, policy =>
            policy.RequireRole(Roles.Administrator));

        options.AddPolicy(Policies.AdminAccess, policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireRole(Roles.Administrator);
        });

        // User policies
        options.AddPolicy(Policies.UserAccess, policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireAssertion(context =>
                context.User.IsInRole(Roles.User) ||
                context.User.IsInRole(Roles.Administrator));
        });

        // Specialized policies
        options.AddPolicy(Policies.ContentManager, policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireRole(Roles.ContentManager);
            policy.RequireClaim("CanEditContent", "true");
        });
    }
}