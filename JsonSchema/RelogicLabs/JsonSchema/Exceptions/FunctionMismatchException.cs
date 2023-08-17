using RelogicLabs.JsonSchema.Message;

namespace RelogicLabs.JsonSchema.Exceptions;

public class FunctionMismatchException : CommonException
{
    public FunctionMismatchException(string code, string message) : base(code, message) { }
    public FunctionMismatchException(ErrorDetail detail) : base(detail) { }
    public FunctionMismatchException(string code, string message, Exception? innerException) 
        : base(code, message, innerException) { }
}