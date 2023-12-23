using RelogicLabs.JsonSchema.Message;

namespace RelogicLabs.JsonSchema.Exceptions;

public class DuplicatePropertyKeyException : CommonException
{
    public DuplicatePropertyKeyException(string code, string message) : base(code, message) { }
    public DuplicatePropertyKeyException(string code, string message, Exception? innerException)
        : base(code, message, innerException) { }
    public DuplicatePropertyKeyException(ErrorDetail detail) : base(detail) { }
    public DuplicatePropertyKeyException(ErrorDetail detail, Exception? innerException)
        : base(detail, innerException) { }
}