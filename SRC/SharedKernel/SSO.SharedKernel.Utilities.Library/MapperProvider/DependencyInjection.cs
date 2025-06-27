using System.Reflection;

namespace SSO.SharedKernel.Utilities.Library.MapperProvider;

public static class DependencyInjection
{
    public static IServiceCollection AddAutoMapperProfiles(
        this IServiceCollection services, 
        IConfiguration configuration,
        Assembly[] assemblies)
    {
        services.AddAutoMapper(assemblies);

        services.AddSingleton<IMapperAdapter, MapperAdapter>();

        return services;
    }
}