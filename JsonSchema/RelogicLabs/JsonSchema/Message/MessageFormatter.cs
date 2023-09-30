using System.Text;
using RelogicLabs.JsonSchema.Tree;
using RelogicLabs.JsonSchema.Utilities;
using static System.Environment;

namespace RelogicLabs.JsonSchema.Message;

// Exception Message Formatter
public abstract class MessageFormatter
{
    private const string SchemaBaseException = "Schema Input [{0}]: {1}";
    private const string JsonBaseException = "Json Input [{0}]: {1}";
    private const string SchemaParseException = "Schema (Line: {0}) [{1}]: {2}";
    private const string JsonParseException = "Json (Line: {0}) [{1}]: {2}";
    
    public static readonly MessageFormatter SchemaValidation
        = new ValidationFormatter
        {
            Summary = "Schema (Line: {0}) Json (Line: {1}) [{2}]: {3}.",
            Expected = " {0} is expected",
            Actual = " but {0}."
        };
    public static readonly MessageFormatter SchemaAssertion
        = new AssertionFormatter
        {
            Summary = $"{{0}}: {{1}}{NewLine}",
            Expected = $"Expected (Schema Line: {{0}}): {{1}}{NewLine}",
            Actual = $"Actual (Json Line: {{0}}): {{1}}{NewLine}"
        };
    
    public static readonly MessageFormatter JsonAssertion
        = new AssertionFormatter
        {
            Summary = $"{{0}}: {{1}}{NewLine}",
            Expected = $"Expected (Json Line: {{0}}): {{1}}{NewLine}",
            Actual = $"Actual (Json Line: {{0}}): {{1}}{NewLine}"
        };

    public required string Summary { get; init; }
    public required string Expected { get; init; }
    public required string Actual { get; init; }
    public int OutlineLength { get; set; } = 200;

    public string CreateOutline(string target)
    {
        int front = 2 * OutlineLength / 3;
        int back = 1 * OutlineLength / 3;
        if(front + back >= target.Length) return target;
        StringBuilder builder = new();
        return builder.Append(target[..front]).Append("...")
            .Append(target[^back..]).ToString();
    }

    private class ValidationFormatter : MessageFormatter
    {
        internal override string Format(ErrorDetail error, ExpectedDetail expected, 
            ActualDetail actual)
        {
            return new StringBuilder()
                .Append(string.Format(Summary, expected.Location, 
                    actual.Location, error.Code, error.Message))
                .Append(string.Format(Expected, expected.Message.ToUpperFirstLetter()))
                .Append(string.Format(Actual, actual.Message))
                .ToString();
        }
    }
    
    private class AssertionFormatter : MessageFormatter
    {
        internal override string Format(ErrorDetail error, ExpectedDetail expected, 
            ActualDetail actual)
        {
            return new StringBuilder()
                .Append(string.Format(Summary, error.Code, error.Message))
                .Append(string.Format(Expected, expected.Location, expected.Message))
                .Append(string.Format(Actual, actual.Location, actual.Message))
                .ToString();
        }
    }

    internal abstract string Format(ErrorDetail error, ExpectedDetail expected,
        ActualDetail actual);

    internal static ErrorDetail FormatForSchema(string code, string message, Context? context)
        => FormatForSchema(code, message, context?.GetLocation());
    
    internal static ErrorDetail FormatForSchema(string code, string message, Location? location) 
        => location == null ? CreateDetail(code, SchemaBaseException, message)
            : CreateDetail(code, SchemaParseException, message, location);

    internal static ErrorDetail FormatForJson(string code, string message, Context? context)
        => FormatForJson(code, message, context?.GetLocation());
    
    internal static ErrorDetail FormatForJson(string code, string message, Location? location) 
        => location == null ? CreateDetail(code, JsonBaseException, message)
            : CreateDetail(code, JsonParseException, message, location);

    private static ErrorDetail CreateDetail(string code, string format, string message) 
        => new(code, string.Format(format, code, message));

    private static ErrorDetail CreateDetail(string code, string format, string message, 
        Location location) => new(code, string.Format(format, location, code, message));
}
