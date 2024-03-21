namespace RelogicLabs.JSchema.Exceptions;

public class JsonParserException : CommonException
{
    public JsonParserException(string code, string message, Exception? innerException = null)
        : base(code, message, innerException) { }
}