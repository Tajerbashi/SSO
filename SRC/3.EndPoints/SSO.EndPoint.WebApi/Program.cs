using SSO.EndPoint.WebApi;
using SSO.EndPoint.WebApi.Providers.Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilogService();

builder.AddWebAPIService();

var app = builder.Build();

await app.InitialiseDatabaseAsync();

app.UseWebAPIService();

app.Run();

