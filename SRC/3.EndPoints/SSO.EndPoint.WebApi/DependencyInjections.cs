using Autofac;
using Autofac.Extensions.DependencyInjection;
using SSO.EndPoint.WebApi.Extensions;
using SSO.EndPoint.WebApi.Providers.Serilog;
using SSO.EndPoint.WebApi.Providers.Swagger;
using SSO.SharedKernel.Utilities.Library.Autofac;
using SSO.SharedKernel.Utilities.Library;
using SSO.EndPoint.WebApi.Providers.Identity;
using SSO.Core.Application.Library;
using SSO.Infra.SQL.Library;
using SSO.Infra.SQL.Library.Context;
using SSO.EndPoint.WebApi.Middleware.ValidationHandler;
using SSO.EndPoint.WebApi.Middleware.ExceptionHandler;

namespace SSO.EndPoint.WebApi;
public static class DependencyInjections
{
    public static WebApplicationBuilder AddWebAPIService(this WebApplicationBuilder builder)
    {
        IConfiguration configuration = builder.Configuration;
        var assemblies = ("SSO").GetAssemblies().ToArray();

        // Configure Autofac
        builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureContainer<ContainerBuilder>(containerBuilder =>
            {
                containerBuilder.AddAutofacLifetimeServices(assemblies);
            });

        // Add Http Context Accessor.
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<ProviderServices>();
        // Add services to the container.
        builder.Services.AddControllers();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        //  Swagger
        builder.Services.AddSwaggerProvider(configuration);

        //  BaseSource Utilities
        builder.Services.AddBaseSourceUtilities(configuration, assemblies);

        builder.Services.AddIdentityConfiguration(configuration, "Identity");

        builder.Services.AddApplicationService(assemblies);

        builder.Services.AddInfrastructureServices(configuration, assemblies);

        return builder;
    }

    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var initialiser = scope.ServiceProvider.GetRequiredService<DataContextInitializer>();
        await initialiser.RunAsync();
    }

    public static WebApplication AddMinimalApis(this WebApplication app)
    {

        var baseUrl = app.Configuration["SSOUrl"] ?? "#";
        var dateTime = DateTime.Now;
        app.MapGet("/", () =>
        {
            var html = HTMLPageExtensions.GenerateStatusHtml("🔐 SSO Server", "#659fff", "#001d4c", baseUrl, dateTime);
            return Results.Content(html, "text/html");
        });

        return app;
    }

    public static WebApplication UseWebAPIService(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();

            //  Swagger
            app.UseSwaggerProvider();
        }
        app.UseHttpsRedirection();

        app.UseMiddleware<RequestLoggingMiddleware>();

        app.UseValidationExceptionHandler();

        app.UseApiExceptionHandler();

        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        return app;
    }
}


