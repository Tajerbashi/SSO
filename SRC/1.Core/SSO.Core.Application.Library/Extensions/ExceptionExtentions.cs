using SSO.Core.Application.Library.Common.Exceptions;

namespace SSO.Core.Application.Library.Extensions;

public static class ExceptionExtentions
{

    public static AppException ThrowException(this Exception exception) => new AppException(exception);
    public static AppException ThrowAppException(this Exception exception) => new AppException(exception.Message, exception);
    public static AppException ThrowAppException(this string message, params string[] parameters) => new AppException(message, parameters);


}
