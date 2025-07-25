using SSO.Core.Domain.Library.Common.Exceptions;

namespace SSO.EndPoint.WebApi.Common.Exceptions;

public class ApiException : BaseException
{
    public ApiException(string message, params string[] parameters) : base(message, parameters)
    {
    }
    public ApiException(Exception exception) : base(exception.Message, exception) { }
}
