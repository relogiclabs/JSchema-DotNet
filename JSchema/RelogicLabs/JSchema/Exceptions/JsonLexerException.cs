namespace RelogicLabs.JSchema.Exceptions;

public class JsonLexerException : CommonException
{
    public JsonLexerException(string code, string message, Exception? innerException = null)
        : base(code, message, innerException) { }
}