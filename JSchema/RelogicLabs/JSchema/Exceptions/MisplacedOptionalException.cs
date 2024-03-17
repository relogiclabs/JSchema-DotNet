using RelogicLabs.JSchema.Message;

namespace RelogicLabs.JSchema.Exceptions;

public class MisplacedOptionalException : CommonException
{
    public MisplacedOptionalException(string code, string message, Exception? innerException = null)
        : base(code, message, innerException) { }
    public MisplacedOptionalException(ErrorDetail detail, Exception? innerException = null)
        : base(detail, innerException) { }
}