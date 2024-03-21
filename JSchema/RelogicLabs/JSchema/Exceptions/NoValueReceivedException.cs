using RelogicLabs.JSchema.Message;

namespace RelogicLabs.JSchema.Exceptions;

public class NoValueReceivedException : CommonException
{
    public NoValueReceivedException(string code, string message, Exception? innerException = null)
        : base(code, message, innerException) { }
    public NoValueReceivedException(ErrorDetail detail, Exception? innerException = null)
        : base(detail, innerException) { }
}