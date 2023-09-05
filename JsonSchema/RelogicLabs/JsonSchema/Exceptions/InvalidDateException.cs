using RelogicLabs.JsonSchema.Message;

namespace RelogicLabs.JsonSchema.Exceptions;

public class InvalidDateException : CommonException
{
    public InvalidDateException(string code, string message) : base(code, message) { }
    public InvalidDateException(ErrorDetail detail) : base(detail) { }
    public InvalidDateException(ErrorDetail detail, Exception? innerException) 
        : base(detail, innerException) { }
    public InvalidDateException(string code, string message, Exception? innerException) 
        : base(code, message, innerException) { }
}