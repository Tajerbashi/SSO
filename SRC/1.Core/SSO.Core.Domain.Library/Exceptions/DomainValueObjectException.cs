﻿using SSO.Core.Domain.Library.Common.Exceptions;

namespace SSO.Core.Domain.Library.Exceptions;

public class DomainValueObjectException : BaseException
{
    public DomainValueObjectException(string message, params string[] parameters) : base(message, parameters)
    {
    }
}
public class DomainLogicException : BaseException
{
    public DomainLogicException(string message, params string[] parameters) : base(message, parameters)
    {
    }
}
