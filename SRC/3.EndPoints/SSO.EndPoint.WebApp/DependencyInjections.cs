using SSO.EndPoint.WebApp.Middleware.ExceptionHandler;
using SSO.EndPoint.WebApp.Middleware.ValidationHandler;
using SSO.EndPoint.WebApp.Providers.IdentityServer;
using SSO.EndPoint.WebApp.Providers.Serilog;

namespace SSO.EndPoint.WebApp;

public static class DependencyInjections
{
    public static WebApplicationBuilder AddMvcRazorPageServices(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        var assemblies = "SSO".GetAssemblies().ToArray();

        builder.AddSerilogService();

        // Configure Autofac
        builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
               .ConfigureContainer<ContainerBuilder>(containerBuilder =>
               {
                   containerBuilder.AddAutofacLifetimeServices(assemblies);
               });

        // Add services
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<ProviderServices>();

        builder.Services.AddRazorPages();
        builder.Services.AddControllersWithViews(); // Needed for hybrid Razor + MVC
        builder.Services.AddServerSideBlazor(); // Optional if using Blazor

        // Swagger (can be optional in Razor Pages app)
        builder.Services.AddSwaggerProvider(configuration);

        // Custom Infrastructure + Identity
        builder.Services.AddBaseSourceUtilities(configuration, assemblies);
        builder.Services.AddIdentityConfiguration(configuration, "Identity");
        builder.Services.AddApplicationService(assemblies);
        builder.Services.AddInfrastructureServices(configuration, assemblies);

        builder.AddIdentityServerConfiguration();

        return builder;
    }

    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var initialiser = scope.ServiceProvider.GetRequiredService<DataContextInitializer>();
        await initialiser.RunAsync();
    }

    public static WebApplication UseMvcRazorPageApp(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwaggerProvider();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.MapStaticAssets();

        app.UseRouting();

        // Custom Middleware
        app.UseMiddleware<RequestLoggingMiddleware>();
        app.UseValidationExceptionHandler();
        app.UseApiExceptionHandler();

        app.UseAuthentication();

        app.UseAuthorization();

        app.UseIdentityServerConfiguration();

        // Razor Pages + MVC
        app.MapRazorPages()
           .WithStaticAssets();

        app.MapDefaultControllerRoute(); // for MVC Controllers

        app.AddMinimalApis();


        return app;
    }

    public static WebApplication AddMinimalApis(this WebApplication app)
    {

        var baseUrl = "/Public/Dashboard";
        var dateTime = DateTime.Now;
        app.MapGet("/", () =>
        {
            var html = HTMLPageExtensions.GenerateStatusHtml("🔐 SSO Portal", "#659fff", "#001d4c", baseUrl, dateTime);
            return Results.Content(html, "text/html");
        });

        return app;
    }

}
