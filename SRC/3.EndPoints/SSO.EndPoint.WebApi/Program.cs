using SSO.EndPoint.WebApi;
using SSO.EndPoint.WebApi.Providers.IdentityServer;
using SSO.EndPoint.WebApi.Providers.Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilogService();

builder.AddWebAPIService();

builder.AddIdentityServerConfiguration();

var app = builder.Build();

await app.InitialiseDatabaseAsync();

app.UseWebAPIService();

app.AddMinimalApis();

app.UseIdentityServerConfiguration();

app.Run();

