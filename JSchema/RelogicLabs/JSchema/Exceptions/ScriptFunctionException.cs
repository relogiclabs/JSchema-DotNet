using RelogicLabs.JSchema.Message;

namespace RelogicLabs.JSchema.Exceptions;

public class ScriptFunctionException : ScriptCommonException
{
    public ScriptFunctionException(string code, string message, Exception? innerException = null)
        : base(code, message, innerException) { }
    public ScriptFunctionException(ErrorDetail detail, Exception? innerException = null)
        : base(detail, innerException) { }
}