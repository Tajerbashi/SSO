using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SSO.EndPoint.WebApi;
using SSO.EndPoint.WebApi.Providers.IdentityServer;
using SSO.EndPoint.WebApi.Providers.Serilog;
using SSO.Infra.SQL.Library.Context;
using SSO.Infra.SQL.Library.Identity.Entities;

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

