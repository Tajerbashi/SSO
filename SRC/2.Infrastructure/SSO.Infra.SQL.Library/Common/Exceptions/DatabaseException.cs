using SSO.Core.Domain.Library.Common.Exceptions;

namespace SSO.Infra.SQL.Library.Common.Exceptions;

internal class DatabaseException : BaseException
{
    public DatabaseException(string message, params string[] parameters) : base(message, parameters)
    {
    }
}
