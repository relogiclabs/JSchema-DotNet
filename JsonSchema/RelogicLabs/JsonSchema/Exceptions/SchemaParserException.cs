namespace RelogicLabs.JsonSchema.Exceptions;

public class SchemaParserException : CommonException
{
    public SchemaParserException(string code, string message) : base(code, message) { }
    public SchemaParserException(string code, string message, Exception? innerException) 
        : base(code, message, innerException) { }
}