using RelogicLabs.JSchema.Utilities;

namespace RelogicLabs.JSchema.Message;

public sealed class ErrorDetail
{
    internal const string ValidationFailed = "Validation failed";
    internal const string ValueMismatch = "Value mismatch";
    internal const string DataTypeMismatch = "Data type mismatch";
    internal const string InvalidNonCompositeType = "Invalid non-composite value type";
    internal const string DataTypeArgumentFailed = "Data type argument failed";
    internal const string InvalidNestedFunction = "Invalid nested function operation";
    internal const string PropertyNotFound = "Mandatory property not found";
    internal const string ArrayElementNotFound = "Mandatory array element not found";
    internal const string UndefinedPropertyFound = "Undefined property found";
    internal const string PropertyKeyMismatch = "Property key mismatch";
    internal const string PropertyValueMismatch = "Property value mismatch";
    internal const string PropertyOrderMismatch = "Property order mismatch";

    public string Code { get; }
    public string Message { get; }

    public ErrorDetail(string code, string message)
    {
        Code = code;
        Message = message.Capitalize();
    }
}