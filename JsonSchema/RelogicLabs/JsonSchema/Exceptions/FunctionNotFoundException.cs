using RelogicLabs.JsonSchema.Message;

namespace RelogicLabs.JsonSchema.Exceptions;

public class FunctionNotFoundException : CommonException
{
    public FunctionNotFoundException(string code, string message) : base(code, message) { }
    public FunctionNotFoundException(ErrorDetail detail) : base(detail) { }
    public FunctionNotFoundException(string code, string message, Exception? innerException) 
        : base(code, message, innerException) { }
}