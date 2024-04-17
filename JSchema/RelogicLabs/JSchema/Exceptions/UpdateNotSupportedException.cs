using RelogicLabs.JSchema.Message;

namespace RelogicLabs.JSchema.Exceptions;

public class UpdateNotSupportedException : CommonException
{
    public UpdateNotSupportedException(string code, string message, Exception? innerException = null)
        : base(code, message, innerException) { }
    protected UpdateNotSupportedException(ErrorDetail detail, Exception? innerException = null)
        : base(detail, innerException) { }
}