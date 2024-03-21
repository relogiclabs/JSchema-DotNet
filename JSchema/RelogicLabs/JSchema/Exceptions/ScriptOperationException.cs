using RelogicLabs.JSchema.Message;

namespace RelogicLabs.JSchema.Exceptions;

public class ScriptOperationException : ScriptRuntimeException
{
    public ScriptOperationException(string code, string message, Exception? innerException = null)
        : base(code, message, innerException) { }
    public ScriptOperationException(ErrorDetail detail, Exception? innerException = null)
        : base(detail, innerException) { }
}