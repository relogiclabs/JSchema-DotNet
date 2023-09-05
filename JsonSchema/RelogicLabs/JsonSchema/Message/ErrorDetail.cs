using System.Diagnostics.CodeAnalysis;

namespace RelogicLabs.JsonSchema.Message;

public class ErrorDetail
{
    internal const string InvalidNestedDataType = "Invalid nested data type operation";
    internal const string InvalidNestedFunction = "Invalid nested function operation";
    internal const string PropertyNotFound = "Mandatory property not found";
    internal const string ArrayElementNotFound = "Mandatory array element not found";
    internal const string UndefinedPropertyFound = "Undefined property found";
    internal const string PropertyKeyMismatch = "Property key mismatch";
    internal const string PropertyValueMismatch = "Property value mismatch";
    internal const string DataTypeMismatch = "Data type mismatch";
    internal const string ValidationFailed = "Validation Failed";
    internal const string ValueMismatch = "Value mismatch";
    internal const string PropertyOrderMismatch = "Property order mismatch";

    public required string Code { get; init; }
    public required string Message { get; init; }
    
    [SetsRequiredMembers]
    public ErrorDetail(string code, string message)
    {
        Code = code;
        Message = message;
    }
}