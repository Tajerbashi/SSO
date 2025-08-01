using NuGet.Configuration;
using SSO.EndPoint.WebApp.Providers.IdentityServer.Models;

namespace SSO.EndPoint.WebApp.Providers.IdentityServer;

public static class IdentityServerExtensions
{
    public static void AddIdentityServerConfiguration(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<IdentityServerOption>(builder.Configuration.GetSection("Identity:IdentityServerConfig"));
        builder.Services.Configure<IdentityServerOption>(builder.Configuration);
        IdentityServerOption options = new IdentityServerOption();
        builder.Configuration.GetSection("Identity:IdentityServerConfig").Bind(options);
        IdentityServerConfig.IdentityServerOption = options;
        builder.Services
            .AddIdentityServer()
            .AddDeveloperSigningCredential()
            .AddInMemoryApiScopes(IdentityServerConfig.ApiScopes)
            .AddInMemoryClients(IdentityServerConfig.Clients)
            .AddInMemoryIdentityResources(IdentityServerConfig.IdentityResources)
            ;
    }


    public static void UseIdentityServerConfiguration(this WebApplication app)
    {
        app.UseIdentityServer();
    }
}
