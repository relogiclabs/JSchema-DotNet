using System.Text.RegularExpressions;

namespace RelogicLabs.JSchema.Exceptions;

public class ScriptTemplateException : ScriptCommonException
{
    private static readonly Regex TemplatePattern = new(" '?~%[0-9]'?", RegexOptions.Compiled);
    public string Template { get; }

    public ScriptTemplateException(string code, string template, Exception? innerException = null)
        : base(code, ToMessage(template), innerException)
    {
        Template = template;
    }

    public string GetMessage(params object[] args)
    {
        // usually ~% does not create any conflicts,
        // but if it does, it only affects the error message
        var index = 0;
        var current = Template;
        foreach(var a in args)
            current = current.Replace("~%" + index++, a.ToString());
        return current;
    }

    private static string ToMessage(string template)
        => TemplatePattern.Replace(template, string.Empty);
}