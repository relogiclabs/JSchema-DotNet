using RelogicLabs.JsonSchema.Message;

namespace RelogicLabs.JsonSchema.Exceptions;

public class InvalidPragmaValueException : CommonException
{
    public InvalidPragmaValueException(string code, string message) : base(code, message) { }
    public InvalidPragmaValueException(ErrorDetail detail) : base(detail) { }
    public InvalidPragmaValueException(string code, string message, Exception? innerException) 
        : base(code, message, innerException) { }
}