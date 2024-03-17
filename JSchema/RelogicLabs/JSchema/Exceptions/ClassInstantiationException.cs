using RelogicLabs.JSchema.Message;

namespace RelogicLabs.JSchema.Exceptions;

public class ClassInstantiationException : CommonException
{
    public ClassInstantiationException(string code, string message, Exception? innerException = null)
        : base(code, message, innerException) { }
    public ClassInstantiationException(ErrorDetail detail, Exception? innerException = null)
        : base(detail, innerException) { }
}