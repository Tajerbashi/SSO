using SSO.EndPoint.WebApp;

var builder = WebApplication.CreateBuilder(args);

builder.AddMvcRazorPageServices();

var app = builder.Build();

await app.InitialiseDatabaseAsync();

app.UseMvcRazorPageApp();

app.Run();




