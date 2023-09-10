namespace RelogicLabs.JsonSchema.Exceptions;

public class DateTimeLexerException : CommonException
{
    public DateTimeLexerException(string code, string message) : base(code, message) { }
    public DateTimeLexerException(string code, string message, Exception? innerException) 
        : base(code, message, innerException) { }
}