using RelogicLabs.JSchema.Message;

namespace RelogicLabs.JSchema.Exceptions;

public class DuplicatePropertyKeyException : CommonException
{
    public DuplicatePropertyKeyException(string code, string message, Exception? innerException = null)
        : base(code, message, innerException) { }
    public DuplicatePropertyKeyException(ErrorDetail detail, Exception? innerException = null)
        : base(detail, innerException) { }
}