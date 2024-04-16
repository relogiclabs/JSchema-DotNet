using System.Diagnostics;
using RelogicLabs.JSchema.Message;
using RelogicLabs.JSchema.Utilities;
using static System.Environment;

namespace RelogicLabs.JSchema.Exceptions;

public class CommonException : Exception
{
    private const string ConstructorName = ".ctor";
    private const string FailMethodPrefix = "Fail";
    // Clear it to get complete stack trace
    public static readonly HashSet<string> SkipModuleStackTrace = new(10);
    private Dictionary<string, string>? _attributes;
    public string Code { get; }
    public override string? StackTrace { get; }

    static CommonException()
    {
        SkipModuleStackTrace.Add("Microsoft.VisualStudio.TestPlatform.TestFramework.dll");
        SkipModuleStackTrace.Add("Microsoft.VisualStudio.TestPlatform.MSTest.TestAdapter.dll");
    }

    protected CommonException(string code, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        Code = code;
        StackTrace = CaptureStackTrace();
    }

    protected CommonException(ErrorDetail detail, Exception? innerException = null)
        : this(detail.Code, detail.Message, innerException) { }

    public string? GetAttribute(string name) => _attributes?.TryGetValue(name);

    public void SetAttribute(string name, string value)
        => (_attributes ??= new Dictionary<string, string>(5))[name] = value;

    private static string CaptureStackTrace()
    {
        var stackTrace = new StackTrace(3, true);
        var frames = stackTrace.GetFrames();
        var start = 0;
        for(var i = start; i < frames.Length; i++)
        {
            if(frames[i].GetMethod()?.Name == ConstructorName) start++;
            else break;
        }
        for(var i = start; i < frames.Length; i++)
        {
            if(frames[i].GetMethod()?.Name.StartsWith(FailMethodPrefix) ?? false) start++;
            else break;
        }
        var end = start;
        for(var i = start; i < frames.Length; i++)
        {
            if(!SkipModuleStackTrace.Contains(frames[i].GetMethod()?.Module.Name
                                              ?? string.Empty)) end++;
            else break;
        }
        return string.Join(NewLine, stackTrace.ToString().Split(NewLine)[start..end]) + NewLine;
    }
}