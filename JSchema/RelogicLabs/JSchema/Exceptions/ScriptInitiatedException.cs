using RelogicLabs.JSchema.Message;

namespace RelogicLabs.JSchema.Exceptions;

public class ScriptInitiatedException : ScriptRuntimeException
{
    public ScriptInitiatedException(string code, string message, Exception? innerException = null)
        : base(code, message, innerException) { }
    public ScriptInitiatedException(ErrorDetail detail, Exception? innerException = null)
        : base(detail, innerException) { }
}