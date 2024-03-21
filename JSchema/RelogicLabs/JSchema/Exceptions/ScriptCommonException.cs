using RelogicLabs.JSchema.Message;

namespace RelogicLabs.JSchema.Exceptions;

public class ScriptCommonException : CommonException
{
    public ScriptCommonException(string code, string message, Exception? innerException = null)
        : base(code, message, innerException) { }
    public ScriptCommonException(ErrorDetail detail, Exception? innerException = null)
        : base(detail, innerException) { }
}