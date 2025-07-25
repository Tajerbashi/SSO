using SSO.EndPoint.WebApi.Common.Exceptions;

namespace SSO.EndPoint.WebApi.Extensions;
public static class ExceptionExtentions
{
    public static ApiException ThrowException(this Exception exception) => new ApiException(exception);

    public static ApiException ThrowApiException(this string message, params string[] parameters)
        => new ApiException(message, parameters);

    public static ApiException ThrowApiException(this Exception exception)
        => new ApiException(exception);
}
