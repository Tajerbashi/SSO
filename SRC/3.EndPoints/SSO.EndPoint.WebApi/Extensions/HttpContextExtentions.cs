using SSO.Core.Application.Library;

namespace SSO.EndPoint.WebApi.Extensions;

public static class HttpContextExtensions
{
    public static ProviderServices ApplicationContext(this HttpContext httpContext) =>
        (ProviderServices)httpContext.RequestServices.GetService(typeof(ProviderServices))!;

}