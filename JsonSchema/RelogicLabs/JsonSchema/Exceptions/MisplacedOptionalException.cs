using RelogicLabs.JsonSchema.Message;

namespace RelogicLabs.JsonSchema.Exceptions;

public class MisplacedOptionalException : CommonException
{
    public MisplacedOptionalException(string code, string message) : base(code, message) { }
    public MisplacedOptionalException(string code, string message, Exception? innerException)
        : base(code, message, innerException) { }
    public MisplacedOptionalException(ErrorDetail detail) : base(detail) { }
    public MisplacedOptionalException(ErrorDetail detail, Exception? innerException)
        : base(detail, innerException) { }
}