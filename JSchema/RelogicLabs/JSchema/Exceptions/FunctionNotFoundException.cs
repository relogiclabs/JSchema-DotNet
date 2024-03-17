using RelogicLabs.JSchema.Message;

namespace RelogicLabs.JSchema.Exceptions;

public class FunctionNotFoundException : CommonException
{
    public FunctionNotFoundException(string code, string message, Exception? innerException = null)
        : base(code, message, innerException) { }
    public FunctionNotFoundException(ErrorDetail detail, Exception? innerException = null)
        : base(detail, innerException) { }
}