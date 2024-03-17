using RelogicLabs.JSchema.Message;

namespace RelogicLabs.JSchema.Exceptions;

public class DefinitionNotFoundException : CommonException
{
    public DefinitionNotFoundException(string code, string message, Exception? innerException = null)
        : base(code, message, innerException) { }
    public DefinitionNotFoundException(ErrorDetail detail, Exception? innerException = null)
        : base(detail, innerException) { }
}