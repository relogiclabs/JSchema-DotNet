using RelogicLabs.JSchema.Message;

namespace RelogicLabs.JSchema.Exceptions;

public class DuplicateDefinitionException : CommonException
{
    public DuplicateDefinitionException(string code, string message, Exception? innerException = null)
        : base(code, message, innerException) { }
    public DuplicateDefinitionException(ErrorDetail detail, Exception? innerException = null)
        : base(detail, innerException) { }
}