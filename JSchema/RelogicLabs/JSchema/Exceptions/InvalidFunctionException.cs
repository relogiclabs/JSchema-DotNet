using RelogicLabs.JSchema.Message;

namespace RelogicLabs.JSchema.Exceptions;

public class InvalidFunctionException : CommonException
{
    public InvalidFunctionException(string code, string message, Exception? innerException = null)
        : base(code, message, innerException) { }
    public InvalidFunctionException(ErrorDetail detail, Exception? innerException = null)
        : base(detail, innerException) { }
}