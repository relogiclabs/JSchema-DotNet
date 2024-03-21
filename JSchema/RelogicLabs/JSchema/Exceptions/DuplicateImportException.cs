using RelogicLabs.JSchema.Message;

namespace RelogicLabs.JSchema.Exceptions;

public class DuplicateImportException : CommonException
{
    public DuplicateImportException(string code, string message, Exception? innerException = null)
        : base(code, message, innerException) { }
    public DuplicateImportException(ErrorDetail detail, Exception? innerException = null)
        : base(detail, innerException) { }
}