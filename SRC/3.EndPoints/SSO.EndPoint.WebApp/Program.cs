using SSO.EndPoint.WebApp;
using SSO.EndPoint.WebApp.Providers.IdentityServer;
using SSO.EndPoint.WebApp.Providers.Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.AddMvcRazorPageServices();

var app = builder.Build();

await app.InitialiseDatabaseAsync();

app.UseMvcRazorPageApp();


app.Run();




