using RelogicLabs.JSchema.Message;

namespace RelogicLabs.JSchema.Exceptions;

public class ScriptVariableException : ScriptCommonException
{
    public ScriptVariableException(string code, string message, Exception? innerException = null)
        : base(code, message, innerException) { }
    public ScriptVariableException(ErrorDetail detail, Exception? innerException = null)
        : base(detail, innerException) { }
}