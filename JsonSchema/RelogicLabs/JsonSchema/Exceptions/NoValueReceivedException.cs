using RelogicLabs.JsonSchema.Message;

namespace RelogicLabs.JsonSchema.Exceptions;

public class NoValueReceivedException : CommonException
{
    public NoValueReceivedException(string code, string message) : base(code, message) { }
    public NoValueReceivedException(string code, string message, Exception? innerException)
        : base(code, message, innerException) { }
    public NoValueReceivedException(ErrorDetail detail) : base(detail) { }
    public NoValueReceivedException(ErrorDetail detail, Exception? innerException)
        : base(detail, innerException) { }
}