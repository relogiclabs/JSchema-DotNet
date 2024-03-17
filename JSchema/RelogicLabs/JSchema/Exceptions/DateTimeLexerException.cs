namespace RelogicLabs.JSchema.Exceptions;

public class DateTimeLexerException : CommonException
{
    public DateTimeLexerException(string code, string message, Exception? innerException = null)
        : base(code, message, innerException) { }
}