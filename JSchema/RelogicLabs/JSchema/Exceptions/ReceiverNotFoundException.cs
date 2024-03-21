using RelogicLabs.JSchema.Message;

namespace RelogicLabs.JSchema.Exceptions;

public class ReceiverNotFoundException : CommonException
{
    public ReceiverNotFoundException(string code, string message, Exception? innerException = null)
        : base(code, message, innerException) { }
    public ReceiverNotFoundException(ErrorDetail detail, Exception? innerException = null)
        : base(detail, innerException) { }
}