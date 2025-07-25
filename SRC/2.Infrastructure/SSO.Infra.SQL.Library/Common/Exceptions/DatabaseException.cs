﻿using SSO.Core.Domain.Library.Common.Exceptions;

namespace SSO.Infra.SQL.Library.Common.Exceptions;

public class DatabaseException : BaseException
{
    public DatabaseException(string message, params string[] parameters) : base(message, parameters)
    {
    }
    public DatabaseException(Exception exception) : base(exception.Message, exception) { }
}
