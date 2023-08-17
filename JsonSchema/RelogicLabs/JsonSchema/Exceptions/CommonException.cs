using RelogicLabs.JsonSchema.Message;

namespace RelogicLabs.JsonSchema.Exceptions;

public class CommonException : Exception
{
    public string ErrorCode { get; }
    
    protected CommonException(string code, string message) : base(message) 
        => ErrorCode = code;
    protected CommonException(ErrorDetail detail) : base(detail.Message)
        => ErrorCode = detail.Code;
    protected CommonException(ErrorDetail detail, Exception? innerException) 
        : base(detail.Message, innerException) => ErrorCode = detail.Code;
    protected CommonException(string code, string message, Exception? innerException) 
        : base(message, innerException) => ErrorCode = code;
}