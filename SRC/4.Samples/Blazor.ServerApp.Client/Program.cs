using Blazor.ServerApp.Client;

var builder = WebApplication.CreateBuilder(args);
var app = builder.AddBuilders().Build();
app.UseApplication().Run();
