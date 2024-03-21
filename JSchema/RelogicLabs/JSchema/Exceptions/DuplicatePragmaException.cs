using RelogicLabs.JSchema.Message;

namespace RelogicLabs.JSchema.Exceptions;

public class DuplicatePragmaException : CommonException
{
    public DuplicatePragmaException(string code, string message, Exception? innerException = null)
        : base(code, message, innerException) { }
    public DuplicatePragmaException(ErrorDetail detail, Exception? innerException = null)
        : base(detail, innerException) { }
}