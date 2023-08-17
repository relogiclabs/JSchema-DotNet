using RelogicLabs.JsonSchema.Message;

namespace RelogicLabs.JsonSchema.Exceptions;

public class DuplicateIncludeException : CommonException
{
    public DuplicateIncludeException(ErrorDetail detail) : base(detail) { }
    public DuplicateIncludeException(string code, string message, Exception? innerException) 
        : base(code, message, innerException) { }
}