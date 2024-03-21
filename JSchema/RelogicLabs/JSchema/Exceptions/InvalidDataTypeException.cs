using RelogicLabs.JSchema.Message;

namespace RelogicLabs.JSchema.Exceptions;

public class InvalidDataTypeException : CommonException
{
    public InvalidDataTypeException(string code, string message, Exception? innerException = null)
        : base(code, message, innerException) { }
    public InvalidDataTypeException(ErrorDetail detail, Exception? innerException = null)
        : base(detail, innerException) { }
}