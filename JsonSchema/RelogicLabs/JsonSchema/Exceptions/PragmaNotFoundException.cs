using RelogicLabs.JsonSchema.Message;

namespace RelogicLabs.JsonSchema.Exceptions;

public class PragmaNotFoundException : CommonException
{
    public PragmaNotFoundException(string code, string message) : base(code, message) { }
    public PragmaNotFoundException(string code, string message, Exception? innerException)
        : base(code, message, innerException) { }
    public PragmaNotFoundException(ErrorDetail detail) : base(detail) { }
    public PragmaNotFoundException(ErrorDetail detail, Exception? innerException)
        : base(detail, innerException) { }
}