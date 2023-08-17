using RelogicLabs.JsonSchema.Message;

namespace RelogicLabs.JsonSchema.Exceptions;

public class DuplicatePragmaException : CommonException
{
    public DuplicatePragmaException(string code, string message) : base(code, message) { }
    public DuplicatePragmaException(ErrorDetail detail) : base(detail) { }
    public DuplicatePragmaException(string code, string message, Exception? innerException) 
        : base(code, message, innerException) { }
}