namespace RelogicLabs.JSchema.Exceptions;

public class ScriptArgumentException : ScriptTemplateException
{
    public ScriptArgumentException(string code, string template, Exception? innerException = null)
        : base(code, template, innerException) { }
}