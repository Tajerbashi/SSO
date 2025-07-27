using SSO.Core.Application.Library.Common.Service;
using SSO.Infra.SQL.Library.Common.Interceptors.ShadowProperties;
using SSO.Infra.SQL.Library.Context;
using SSO.Infra.SQL.Library.Identity.Entities;

namespace SSO.Infra.SQL.Library;

public static class DependencyInjections
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration, Assembly[] assemblies)
    {
        services.AddScoped<ISaveChangesInterceptor, AddAuditDataInterceptor>();

        services.AddDbContext<DataContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(DataContext).Assembly.FullName);
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                });

            options.AddInterceptors(new AddAuditDataInterceptor());
            //options.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll); // default, can omit if preferred
        });

        //services.AddDefaultIdentity<UserIdentity>(
        //    options => options.SignIn.RequireConfirmedAccount = true)
        //    .AddEntityFrameworkStores<DataContext>();


        services.AddScoped<DataContextInitializer>();

        services.AddRepositories(assemblies);

        return services;
    }

    /// <summary>
    /// Scrutor
    /// </summary>
    /// <param name="services"></param>
    /// <param name="assemblies"></param>
    /// <returns></returns>
    public static IServiceCollection AddRepositories(this IServiceCollection services, Assembly[] assemblies)
    {
        services.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(classes => classes.Where(type => type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandRepository<,>))))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(classes => classes.Where(type => type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQueryRepository<,>))))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services, Assembly[] assemblies)
    {
        services
            .Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(classes => classes.Where(type => type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandService<,>))))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services
            .Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(classes => classes.Where(type => type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQueryService<,>))))
            .AsImplementedInterfaces()
            .WithScopedLifetime());



        return services;
    }


}
