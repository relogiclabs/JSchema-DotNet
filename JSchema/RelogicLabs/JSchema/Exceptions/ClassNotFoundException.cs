using RelogicLabs.JSchema.Message;

namespace RelogicLabs.JSchema.Exceptions;

public class ClassNotFoundException : CommonException
{
    public ClassNotFoundException(string code, string message, Exception? innerException = null)
        : base(code, message, innerException) { }
    public ClassNotFoundException(ErrorDetail detail, Exception? innerException = null)
        : base(detail, innerException) { }
}