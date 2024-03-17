namespace RelogicLabs.JSchema.Exceptions;

public class SchemaParserException : CommonException
{
    public SchemaParserException(string code, string message, Exception? innerException = null)
        : base(code, message, innerException) { }
}