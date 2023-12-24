using RelogicLabs.JsonSchema.Message;

namespace RelogicLabs.JsonSchema.Exceptions;

public class InvalidPragmaValueException : CommonException
{
    public InvalidPragmaValueException(string code, string message) : base(code, message) { }
    public InvalidPragmaValueException(string code, string message, Exception? innerException)
        : base(code, message, innerException) { }
    public InvalidPragmaValueException(ErrorDetail detail) : base(detail) { }
    public InvalidPragmaValueException(ErrorDetail detail, Exception? innerException)
        : base(detail, innerException) { }
}