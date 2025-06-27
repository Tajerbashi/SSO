using Microsoft.Extensions.DependencyInjection;

namespace SSO.SharedKernel.Utilities.Library.SerializerProvider;

public static class DependencyInjection
{
    public static IServiceCollection AddMicrosoftSerializer(this IServiceCollection services)
    => services.AddSingleton<IJsonSerializer, MicrosoftSerializer>();
}