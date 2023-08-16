using System.Text;
using RelogicLabs.JsonSchema.Tree;
using RelogicLabs.JsonSchema.Utilities;
using static System.Environment;

namespace RelogicLabs.JsonSchema.Message;

// Exception Message Formatter
internal abstract class MessageFormatter
{
    private const string SchemaBaseException = "Schema Input [{0}]: {1}";
    private const string JsonBaseException = "Json Input [{0}]: {1}";
    private const string SchemaParseException = "Schema (Line: {0}) [{1}]: {2}";
    private const string JsonParseException = "Json (Line: {0}) [{1}]: {2}";
    
    public static readonly MessageFormatter SchemaValidation
        = new SchemaValidationFormatter
        {
            Header = "Schema (Line: {0}) Json (Line: {1}) [{2}]: {3}.",
            Expected = " {0} is expected",
            Actual = " but {0}."
        };
    public static readonly MessageFormatter SchemaAssertion
        = new SchemaAssertionFormatter
        {
            Header = $"{{0}}: {{1}}{NewLine}",
            Expected = $"Expected (Schema Line: {{0}}): {{1}}{NewLine}",
            Actual = $"Actual (Json Line: {{0}}): {{1}}{NewLine}"
        };
    
    public static readonly MessageFormatter JsonAssertion
        = new JsonAssertionFormatter
        {
            Header = $"{{0}}: {{1}}{NewLine}",
            Expected = $"Expected (Json Line: {{0}}): {{1}}{NewLine}",
            Actual = $"Actual (Json Line: {{0}}): {{1}}{NewLine}"
        };

    public required string Header { get; init; }
    public required string Expected { get; init; }
    public required string Actual { get; init; }
    public int OutlineLength { get; init; } = 200;


    private class SchemaValidationFormatter : MessageFormatter
    {
        internal override string Format(ErrorDetail error, ExpectedDetail expected, 
            ActualDetail actual)
        {
            return new StringBuilder()
                .Append(string.Format(Header, expected.Location, 
                    actual.Location, error.Code, error.Message))
                .Append(string.Format(Expected, expected.Message.ToUpperFirstLetter()))
                .Append(string.Format(Actual, actual.Message))
                .ToString();
        }
    }
    
    private class SchemaAssertionFormatter : MessageFormatter
    {
        internal override string Format(ErrorDetail error, ExpectedDetail expected, 
            ActualDetail actual)
        {
            return new StringBuilder()
                .Append(string.Format(Header, error.Code, error.Message))
                .Append(string.Format(Expected, expected.Location, expected.Message))
                .Append(string.Format(Actual, actual.Location, actual.Message))
                .ToString();
        }
    }

    private class JsonAssertionFormatter : MessageFormatter
    {
        internal override string Format(ErrorDetail error, ExpectedDetail expected, 
            ActualDetail actual)
        {
            return new StringBuilder()
                .Append(string.Format(Header, error.Code, error.Message))
                .Append(string.Format(Expected, expected.Location, expected.Message))
                .Append(string.Format(Actual, actual.Location, actual.Message))
                .ToString();
        }
    }

    internal abstract string Format(ErrorDetail error, ExpectedDetail expected,
        ActualDetail actual);

    internal static ErrorDetail FormatForSchema(string code, string message, 
        Context? context = null) 
        => context == null ? CreateDetail(code, SchemaBaseException, message)
            : CreateDetail(code, SchemaParseException, message, context);

    internal static ErrorDetail FormatForJson(string code, string message, 
        Context? context = null) 
        => context == null ? CreateDetail(code, JsonBaseException, message)
            : CreateDetail(code, JsonParseException, message, context);

    private static ErrorDetail CreateDetail(string code, string format, string message) 
        => new(code, string.Format(format, code, message));

    private static ErrorDetail CreateDetail(string code, string format, string message, 
        Context context) => new(code, string.Format(format, context.GetLocation(), 
            code, message));
}