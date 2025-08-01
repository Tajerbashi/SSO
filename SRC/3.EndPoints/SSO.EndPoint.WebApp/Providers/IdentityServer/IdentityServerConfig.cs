using Duende.IdentityServer.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SSO.EndPoint.WebApp.Providers.IdentityServer.Models;

namespace SSO.EndPoint.WebApp.Providers.IdentityServer;

public static class IdentityServerConfig
{
    public static IdentityServerOption IdentityServerOption { get; set; }
    public static IEnumerable<IdentityResource> IdentityResources =>
        new List<IdentityResource>
        {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
        };

    public static IEnumerable<ApiScope> ApiScopes
        => IdentityServerOption.Users
        .Select(item => new ApiScope(item.ClientId, item.Name))
        .ToList();
    #region ApiScope Comment
    //public static IEnumerable<ApiScope> ApiScopes =>
    //    new List<ApiScope>
    //    {
    //            new ApiScope("shopapi", "Shop API"),
    //            new ApiScope("supportapi", "Support API")
    //    };
    #endregion

    public static IEnumerable<Client> Clients
        => IdentityServerOption.Users.Select(item => item.GetClient()).ToList();
    #region Client Comment
    //public static IEnumerable<Client> Clients
    //    => IdentityServerOption.Users
    //    .Select(item => new Client()
    //    {
    //        ClientId = item.ClientId,
    //        ClientName = item.Name,
    //        ClientSecrets = { new Secret("secret".Sha256()) },
    //        AllowedGrantTypes = GetAllowedGrantTypes(item.AllowedGrantTypes),
    //        RedirectUris = {
    //            $"{item.Uri}/signin-oidc",
    //            $"{item.Uri}/authentication/login-callback"
    //        },
    //        PostLogoutRedirectUris = {
    //            $"{item.Uri}/signout-callback-oidc"
    //        },
    //        AllowedScopes = { "openid", "profile", "email", "shopapi", "supportapi" },
    //        RequirePkce = true,
    //        AllowOfflineAccess = true,
    //        RequireConsent = false,
    //        AlwaysIncludeUserClaimsInIdToken = true
    //    })
    //    .ToList();

    //public static IEnumerable<Client> Clients =>
    //    new List<Client>
    //    {
    //            // Blazor Server Client
    //            new Client
    //            {
    //                ClientId = "blazorserver",
    //                ClientName = "Blazor Server Client",
    //                ClientSecrets = { new Secret("secret".Sha256()) },
    //                AllowedGrantTypes = GrantTypes.Code,
    //                RedirectUris = {
    //                    "https://localhost:7200/signin-oidc",
    //                    "https://localhost:7200/authentication/login-callback"
    //                },
    //                PostLogoutRedirectUris = {
    //                    "https://localhost:7200/signout-callback-oidc"
    //                },
    //                AllowedScopes = { "openid", "profile", "email", "shopapi", "supportapi" },
    //                RequirePkce = true,
    //                AllowOfflineAccess = true,
    //                RequireConsent = false,
    //                AlwaysIncludeUserClaimsInIdToken = true
    //            },


    //            // MVC Client
    //            new Client
    //            {
    //                ClientId = "mvcclient",
    //                ClientSecrets = { new Secret("secret".Sha256()) },
    //                AllowedGrantTypes = GrantTypes.Code,
    //                RedirectUris = { "https://localhost:7400/signin-oidc" },
    //                PostLogoutRedirectUris = { "https://localhost:7400/signout-callback-oidc" },
    //                AllowedScopes = { "openid", "profile", "email", "shopapi", "supportapi" },
    //                RequirePkce = true,
    //                AllowOfflineAccess = true
    //            },

    //            // API Gateway Client
    //            new Client
    //            {
    //                ClientId = "apigateway",
    //                ClientSecrets = { new Secret("apigatewaysecret".Sha256()) },
    //                AllowedGrantTypes = GrantTypes.ClientCredentials,
    //                AllowedScopes = { "shopapi", "supportapi" }
    //            }
    //    };
    #endregion

    private static ICollection<string> GetAllowedGrantTypes(this string value)
    {
        return value switch
        {
            "User" => GrantTypes.Hybrid,
            "Server" => GrantTypes.Code,
            "Gateway" => GrantTypes.ClientCredentials,
            "" => GrantTypes.Hybrid
        };
    }
    private static Client GetClient(this IdentityUserConfig userConfig)
    {
        if (userConfig.AllowedGrantTypes == "User" || userConfig.AllowedGrantTypes == "Server")
        {
            return new Client
            {
                ClientId = userConfig.ClientId,
                ClientName = userConfig.Name,
                AllowedGrantTypes = userConfig.AllowedGrantTypes.GetAllowedGrantTypes(),
                RequireClientSecret = true,
                ClientSecrets = { new Secret("secret".Sha256()) },
                RedirectUris = { $"{userConfig.Uri}/signin-oidc" },
                PostLogoutRedirectUris = { $"{userConfig.Uri}/signout-callback-oidc" },
                AllowedScopes = { "openid", "profile", "email", "shopapi", "supportapi" },
            };
        }
        else if (userConfig.AllowedGrantTypes == "Gateway")
        {
            return new Client
            {
                ClientId = userConfig.ClientId,
                ClientName = userConfig.Name,
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret("secret".Sha256()) },
                AllowedScopes = { "shopapi", "supportapi" }
            };
        }
        return new Client();
    }
}
