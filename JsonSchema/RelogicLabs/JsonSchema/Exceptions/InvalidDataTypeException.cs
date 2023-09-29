using RelogicLabs.JsonSchema.Message;

namespace RelogicLabs.JsonSchema.Exceptions;

public class InvalidDataTypeException : CommonException
{
    public InvalidDataTypeException(string code, string message) : base(code, message) { }
    public InvalidDataTypeException(ErrorDetail detail) : base(detail) { }
    public InvalidDataTypeException(ErrorDetail detail, Exception? innerException) 
        : base(detail, innerException) { }
    public InvalidDataTypeException(string code, string message, Exception? innerException) 
        : base(code, message, innerException) { }
}