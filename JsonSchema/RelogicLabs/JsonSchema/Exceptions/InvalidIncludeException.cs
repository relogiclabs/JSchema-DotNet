using RelogicLabs.JsonSchema.Message;

namespace RelogicLabs.JsonSchema.Exceptions;

public class InvalidIncludeException : CommonException
{
    public InvalidIncludeException(string code, string message) : base(code, message) { }
    public InvalidIncludeException(ErrorDetail detail) : base(detail) { }
    public InvalidIncludeException(string code, string message, Exception? innerException) 
        : base(code, message, innerException) { }
}