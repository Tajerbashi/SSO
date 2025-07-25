using SSO.Core.Domain.Library.Common.Exceptions;

namespace SSO.Core.Application.Library.Common.Exceptions;

public class AppException : BaseException
{
    public AppException(string message) : base(message)
    {
    }

    public AppException(string message, Exception exception) : base(message, exception)
    {
    }
    public AppException(Exception exception) : base(exception.Message, exception)
    {
    }

    public AppException(string message, params string[] parameters) : base(message, parameters)
    {
    }
}
