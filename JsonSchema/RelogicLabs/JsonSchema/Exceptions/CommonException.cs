using RelogicLabs.JsonSchema.Message;

namespace RelogicLabs.JsonSchema.Exceptions;

public class CommonException : Exception
{
    public string Code { get; }
    
    protected CommonException(string code, string message) : base(message) 
        => Code = code;
    protected CommonException(ErrorDetail detail) : base(detail.Message)
        => Code = detail.Code;
    protected CommonException(ErrorDetail detail, Exception? innerException) 
        : base(detail.Message, innerException) => Code = detail.Code;
    protected CommonException(string code, string message, Exception? innerException) 
        : base(message, innerException) => Code = code;
}