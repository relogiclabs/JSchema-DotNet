using RelogicLabs.JsonSchema.Message;

namespace RelogicLabs.JsonSchema.Exceptions;

public class DuplicateIncludeException : CommonException
{
    public DuplicateIncludeException(string code, string message) : base(code, message) { }
    public DuplicateIncludeException(string code, string message, Exception? innerException)
        : base(code, message, innerException) { }
    public DuplicateIncludeException(ErrorDetail detail) : base(detail) { }
    public DuplicateIncludeException(ErrorDetail detail, Exception? innerException)
        : base(detail, innerException) { }
}