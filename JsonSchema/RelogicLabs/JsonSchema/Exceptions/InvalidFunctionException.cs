using RelogicLabs.JsonSchema.Message;

namespace RelogicLabs.JsonSchema.Exceptions;

public class InvalidFunctionException : CommonException
{
    public InvalidFunctionException(string code, string message) : base(code, message) { }
    public InvalidFunctionException(ErrorDetail detail) : base(detail) { }
    public InvalidFunctionException(string code, string message, Exception? innerException) 
        : base(code, message, innerException) { }
}