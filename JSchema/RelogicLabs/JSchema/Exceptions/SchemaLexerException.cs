namespace RelogicLabs.JsonSchema.Exceptions;

public class SchemaLexerException : CommonException
{
    public SchemaLexerException(string code, string message) : base(code, message) { }
    public SchemaLexerException(string code, string message, Exception? innerException) 
        : base(code, message, innerException) { }
}