using RelogicLabs.JsonSchema.Message;

namespace RelogicLabs.JsonSchema.Exceptions;

public class NoValueReceivedException : CommonException
{
    public NoValueReceivedException(string code, string message) : base(code, message) { }
    public NoValueReceivedException(ErrorDetail detail) : base(detail) { }
    public NoValueReceivedException(string code, string message, Exception? innerException)
        : base(code, message, innerException) { }
}