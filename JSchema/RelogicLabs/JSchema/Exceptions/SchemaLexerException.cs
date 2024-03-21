namespace RelogicLabs.JSchema.Exceptions;

public class SchemaLexerException : CommonException
{
    public SchemaLexerException(string code, string message, Exception? innerException = null)
        : base(code, message, innerException) { }
}