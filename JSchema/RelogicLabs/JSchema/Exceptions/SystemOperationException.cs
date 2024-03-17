using RelogicLabs.JSchema.Message;

namespace RelogicLabs.JSchema.Exceptions;

public class SystemOperationException : ScriptRuntimeException
{
    public SystemOperationException(string code, string message, Exception? innerException = null)
        : base(code, message, innerException) { }
    public SystemOperationException(ErrorDetail detail, Exception? innerException = null)
        : base(detail, innerException) { }
}