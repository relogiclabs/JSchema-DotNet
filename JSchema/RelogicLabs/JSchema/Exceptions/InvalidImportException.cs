using RelogicLabs.JSchema.Message;

namespace RelogicLabs.JSchema.Exceptions;

public class InvalidImportException : CommonException
{
    public InvalidImportException(string code, string message, Exception? innerException = null)
        : base(code, message, innerException) { }
    public InvalidImportException(ErrorDetail detail, Exception? innerException = null)
        : base(detail, innerException) { }
}