using RelogicLabs.JsonSchema.Message;

namespace RelogicLabs.JsonSchema.Exceptions;

public class InvalidDateTimeException : CommonException
{
    public InvalidDateTimeException(string code, string message) : base(code, message) { }
    public InvalidDateTimeException(string code, string message, Exception? innerException)
        : base(code, message, innerException) { }
    public InvalidDateTimeException(ErrorDetail detail) : base(detail) { }
    public InvalidDateTimeException(ErrorDetail detail, Exception? innerException)
        : base(detail, innerException) { }
}