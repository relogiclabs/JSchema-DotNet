using RelogicLabs.JSchema.Message;

namespace RelogicLabs.JSchema.Exceptions;

public class MethodNotFoundException : ScriptRuntimeException
{
    public MethodNotFoundException(string code, string message, Exception? innerException = null)
        : base(code, message, innerException) { }
    public MethodNotFoundException(ErrorDetail detail, Exception? innerException = null)
        : base(detail, innerException) { }
}