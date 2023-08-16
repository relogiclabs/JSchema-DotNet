namespace RelogicLabs.JsonSchema.Exceptions;

public class JsonParserException : CommonException
{
    public JsonParserException(string code, string message) : base(code, message) { }
    public JsonParserException(string code, string message, Exception? innerException) 
        : base(code, message, innerException) { }
}