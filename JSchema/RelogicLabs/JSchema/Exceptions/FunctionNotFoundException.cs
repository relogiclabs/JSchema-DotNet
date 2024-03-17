using RelogicLabs.JsonSchema.Message;

namespace RelogicLabs.JsonSchema.Exceptions;

public class FunctionNotFoundException : CommonException
{
    public FunctionNotFoundException(string code, string message) : base(code, message) { }
    public FunctionNotFoundException(string code, string message, Exception? innerException)
        : base(code, message, innerException) { }
    public FunctionNotFoundException(ErrorDetail detail) : base(detail) { }
    public FunctionNotFoundException(ErrorDetail detail, Exception? innerException)
        : base(detail, innerException) { }
}