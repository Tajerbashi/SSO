using SSO.SharedKernel.Utilities.Library.CacheProvider;
using SSO.SharedKernel.Utilities.Library.DapperProvider;
using SSO.SharedKernel.Utilities.Library.MapperProvider;
using SSO.SharedKernel.Utilities.Library.Scrutor;
using SSO.SharedKernel.Utilities.Library.SerializerProvider;

namespace SSO.SharedKernel.Utilities.Library;

public static class DependencyInjections
{
    public static IServiceCollection AddBaseSourceUtilities(this IServiceCollection services, IConfiguration configuration, Assembly[] assemblies)
    {
        services.AddMicrosoftSerializer();

        services.AddQueryExecute(configuration);

        services.AddScrutorProvider(assemblies);

        services.AddAutoMapperProfiles(configuration, assemblies);

        services.AddSqlDistributedCache(configuration, "SqlCache");

        return services;
    }
}

