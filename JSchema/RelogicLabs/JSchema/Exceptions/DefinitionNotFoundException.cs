using RelogicLabs.JsonSchema.Message;

namespace RelogicLabs.JsonSchema.Exceptions;

public class DefinitionNotFoundException : CommonException
{
    public DefinitionNotFoundException(string code, string message) : base(code, message) { }
    public DefinitionNotFoundException(string code, string message, Exception? innerException)
        : base(code, message, innerException) { }
    public DefinitionNotFoundException(ErrorDetail detail) : base(detail) { }
    public DefinitionNotFoundException(ErrorDetail detail, Exception? innerException)
        : base(detail, innerException) { }
}