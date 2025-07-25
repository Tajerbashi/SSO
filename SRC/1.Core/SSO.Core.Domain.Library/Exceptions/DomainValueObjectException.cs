using SSO.Core.Domain.Library.Common.Exceptions;

namespace SSO.Core.Domain.Library.Exceptions;

public class DomainValueObjectException : BaseException
{
    public DomainValueObjectException(string message, params string[] parameters) : base(message, parameters)
    {
    }
    public DomainValueObjectException(Exception exception) : base(exception.Message, exception)
    {

    }
}
public class DomainLogicException : BaseException
{
    public DomainLogicException(string message, params string[] parameters) : base(message, parameters)
    {
    }
    public DomainLogicException(Exception exception) : base(exception.Message, exception)
    { }
}

public class DomainException : BaseException
{
    public DomainException(Exception exception) : base(exception.Message, exception)
    {
    }
}