using Duende.IdentityServer.Models;

namespace SSO.EndPoint.WebApi.Providers.IdentityServer;

public static class IdentityServerConfig
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new List<IdentityResource>
        {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>
        {
                new ApiScope("shopapi", "Shop API"),
                new ApiScope("supportapi", "Support API")
        };

    public static IEnumerable<Client> Clients =>
        new List<Client>
        {
                // Blazor Server Client
                new Client
                {
                    ClientId = "blazorserver",
                    ClientName = "Blazor Server Client",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = {
                        "https://localhost:7200/signin-oidc",
                        "https://localhost:7200/authentication/login-callback"
                    },
                    PostLogoutRedirectUris = {
                        "https://localhost:7200/signout-callback-oidc"
                    },
                    AllowedScopes = { "openid", "profile", "email", "shopapi", "supportapi" },
                    RequirePkce = true,
                    AllowOfflineAccess = true,
                    RequireConsent = false,
                    AlwaysIncludeUserClaimsInIdToken = true
                },

                
                // MVC Client
                new Client
                {
                    ClientId = "mvcclient",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = { "https://localhost:7400/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:7400/signout-callback-oidc" },
                    AllowedScopes = { "openid", "profile", "email", "shopapi", "supportapi" },
                    RequirePkce = true,
                    AllowOfflineAccess = true
                },
                
                // API Gateway Client
                new Client
                {
                    ClientId = "apigateway",
                    ClientSecrets = { new Secret("apigatewaysecret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = { "shopapi", "supportapi" }
                }
        };
}

public static class IdentityServerExtensions
{
    public static void AddIdentityServerConfiguration(this WebApplicationBuilder builder)
    {

        builder.Services
            .AddIdentityServer()
            .AddInMemoryIdentityResources(IdentityServerConfig.IdentityResources)
            .AddInMemoryApiScopes(IdentityServerConfig.ApiScopes)
            .AddInMemoryClients(IdentityServerConfig.Clients)
            .AddDeveloperSigningCredential();
    }


    public static void UseIdentityServerConfiguration(this WebApplication app)
    {
        app.UseIdentityServer();
    }
}
