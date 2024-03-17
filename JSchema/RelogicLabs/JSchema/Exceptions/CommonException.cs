using RelogicLabs.JsonSchema.Message;
using RelogicLabs.JsonSchema.Utilities;

namespace RelogicLabs.JsonSchema.Exceptions;

public class CommonException : Exception
{
    private Dictionary<string, string>? _attributes;
    public string Code { get; }

    protected CommonException(string code, string message) : base(message)
        => Code = code;
    protected CommonException(string code, string message, Exception? innerException)
        : base(message, innerException) => Code = code;
    protected CommonException(ErrorDetail detail) : base(detail.Message)
        => Code = detail.Code;
    protected CommonException(ErrorDetail detail, Exception? innerException)
        : base(detail.Message, innerException) => Code = detail.Code;

    public string? GetAttribute(string name) => _attributes?.TryGetValue(name);

    public void SetAttribute(string name, string value)
        => (_attributes ??= new Dictionary<string, string>(5))[name] = value;
}