namespace RelogicLabs.JsonSchema.Exceptions;

public class DateLexerException : CommonException
{
    public DateLexerException(string code, string message) : base(code, message) { }
    public DateLexerException(string code, string message, Exception? innerException) 
        : base(code, message, innerException) { }
}