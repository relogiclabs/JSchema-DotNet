namespace RelogicLabs.JsonSchema.Exceptions;

public class JsonLexerException: CommonException
{
    public JsonLexerException(string code, string message) : base(code, message) { }
    public JsonLexerException(string code, string message, Exception? innerException) 
        : base(code, message, innerException) { }
}