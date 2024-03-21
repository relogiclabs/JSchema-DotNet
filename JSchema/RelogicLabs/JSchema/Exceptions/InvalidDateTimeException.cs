using RelogicLabs.JSchema.Message;

namespace RelogicLabs.JSchema.Exceptions;

public class InvalidDateTimeException : CommonException
{
    public InvalidDateTimeException(string code, string message, Exception? innerException = null)
        : base(code, message, innerException) { }
    public InvalidDateTimeException(ErrorDetail detail, Exception? innerException = null)
        : base(detail, innerException) { }
}