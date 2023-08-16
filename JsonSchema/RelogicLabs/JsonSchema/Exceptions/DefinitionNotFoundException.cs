using RelogicLabs.JsonSchema.Message;

namespace RelogicLabs.JsonSchema.Exceptions;

public class DefinitionNotFoundException : CommonException
{
    public DefinitionNotFoundException(string code, string message) : base(code, message) { }
    public DefinitionNotFoundException(ErrorDetail detail) : base(detail) { }
    public DefinitionNotFoundException(string code, string message, Exception? innerException) 
        : base(code, message, innerException) { }
}