using RelogicLabs.JSchema.Message;

namespace RelogicLabs.JSchema.Exceptions;

public class InvalidPragmaValueException : CommonException
{
    public InvalidPragmaValueException(string code, string message, Exception? innerException = null)
        : base(code, message, innerException) { }
    public InvalidPragmaValueException(ErrorDetail detail, Exception? innerException = null)
        : base(detail, innerException) { }
}