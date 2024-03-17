using RelogicLabs.JSchema.Message;

namespace RelogicLabs.JSchema.Exceptions;

public class ScriptRuntimeException : ScriptCommonException
{
    public ScriptRuntimeException(string code, string message, Exception? innerException = null)
        : base(code, message, innerException) { }
    public ScriptRuntimeException(ErrorDetail detail, Exception? innerException = null)
        : base(detail, innerException) { }
}