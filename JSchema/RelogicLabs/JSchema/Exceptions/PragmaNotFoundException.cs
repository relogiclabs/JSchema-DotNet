using RelogicLabs.JSchema.Message;

namespace RelogicLabs.JSchema.Exceptions;

public class PragmaNotFoundException : CommonException
{
    public PragmaNotFoundException(string code, string message, Exception? innerException = null)
        : base(code, message, innerException) { }
    public PragmaNotFoundException(ErrorDetail detail, Exception? innerException = null)
        : base(detail, innerException) { }
}