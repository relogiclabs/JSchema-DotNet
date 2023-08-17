using RelogicLabs.JsonSchema.Message;

namespace RelogicLabs.JsonSchema.Exceptions;

public class ClassInstantiationException : CommonException
{
    public ClassInstantiationException(string code, string message) : base(code, message) { }
    public ClassInstantiationException(ErrorDetail detail) : base(detail) { }
    public ClassInstantiationException(ErrorDetail detail, Exception? innerException) 
        : base(detail, innerException) { }
    public ClassInstantiationException(string code, string message, Exception? innerException) 
        : base(code, message, innerException) { }
}