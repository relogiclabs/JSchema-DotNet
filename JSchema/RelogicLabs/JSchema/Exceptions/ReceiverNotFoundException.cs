using RelogicLabs.JsonSchema.Message;

namespace RelogicLabs.JsonSchema.Exceptions;

public class ReceiverNotFoundException : CommonException
{
    public ReceiverNotFoundException(string code, string message) : base(code, message) { }
    public ReceiverNotFoundException(string code, string message, Exception? innerException)
        : base(code, message, innerException) { }
    public ReceiverNotFoundException(ErrorDetail detail) : base(detail) { }
    public ReceiverNotFoundException(ErrorDetail detail, Exception? innerException)
        : base(detail, innerException) { }
}