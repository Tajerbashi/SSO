using SSO.Core.Domain.Library.Exceptions;

namespace SSO.Core.Domain.Library.Extensions;

public static class ExceptionExtentions
{
    public static DomainException ThrowException(this Exception exception) => new DomainException(exception);
    
    public static DomainLogicException ThrowLogicExceptino(this string message, params string[] parameters) 
        => new DomainLogicException(message, parameters);
    
    public static DomainLogicException ThrowLogicExceptino(this Exception exception) 
        => new DomainLogicException(exception);


    public static DomainValueObjectException ThrowValueObjectException(this string message, params string[] parameters) 
        => new DomainValueObjectException(message, parameters);
    
    public static DomainValueObjectException ThrowValueObjectException(this Exception exception) 
        => new DomainValueObjectException(exception);
}
