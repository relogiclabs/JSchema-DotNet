using System.Text;
using RelogicLabs.JsonSchema.Tree;
using RelogicLabs.JsonSchema.Types;
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
        = new ValidationFormatter(
            "Schema (Line: {0}) Json (Line: {1}) [{2}]: {3}.",
            " {0} is expected",
            " but {0}.");

    public static readonly MessageFormatter SchemaAssertion
        = new AssertionFormatter(
            $"{{0}}: {{1}}{NewLine}",
            $"Expected (Schema Line: {{0}}): {{1}}{NewLine}",
            $"Actual (Json Line: {{0}}): {{1}}{NewLine}");

    public static readonly MessageFormatter JsonAssertion
        = new AssertionFormatter(
            $"{{0}}: {{1}}{NewLine}",
            $"Expected (Json Line: {{0}}): {{1}}{NewLine}",
            $"Actual (Json Line: {{0}}): {{1}}{NewLine}");

    public string Summary { get; }
    public string Expected { get; }
    public string Actual { get; }
    public int OutlineLength { get; set; } = 200;

    private MessageFormatter(string summary, string expected, string actual)
    {
        Summary = summary;
        Expected = expected;
        Actual = actual;
    }

    public string CreateOutline(string target)
    {
        int front = 2 * OutlineLength / 3;
        int back = 1 * OutlineLength / 3;
        if(front + back >= target.Length) return target;
        StringBuilder builder = new();
        return builder.Append(target[..front]).Append("...")
            .Append(target[^back..]).ToString();
    }

    private sealed class ValidationFormatter : MessageFormatter
    {
        public ValidationFormatter(string summary, string expected, string actual)
            : base(summary, expected, actual) { }

        internal override string Format(ErrorDetail error, ExpectedDetail expected,
            ActualDetail actual)
        {
            return new StringBuilder()
                .Append(string.Format(Summary, expected.Location,
                    actual.Location, error.Code, error.Message))
                .Append(string.Format(Expected, expected.Message.Capitalize()))
                .Append(string.Format(Actual, actual.Message))
                .ToString();
        }
    }

    private sealed class AssertionFormatter : MessageFormatter
    {
        public AssertionFormatter(string summary, string expected, string actual)
            : base(summary, expected, actual) { }

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

    internal static ErrorDetail FormatForSchema(string code, string message, JNode? node)
        => FormatForSchema(code, message, node?.Context);

    internal static ErrorDetail FormatForSchema(string code, string message, Context? context)
        => FormatForSchema(code, message, context?.GetLocation());

    internal static ErrorDetail FormatForSchema(string code, string message, Location? location)
        => location == null ? CreateDetail(code, SchemaBaseException, message)
            : CreateDetail(code, SchemaParseException, message, (Location) location);

    internal static ErrorDetail FormatForJson(string code, string message, JNode? node)
        => FormatForJson(code, message, node?.Context);

    internal static ErrorDetail FormatForJson(string code, string message, Context? context)
        => FormatForJson(code, message, context?.GetLocation());

    internal static ErrorDetail FormatForJson(string code, string message, Location? location)
        => location == null ? CreateDetail(code, JsonBaseException, message)
            : CreateDetail(code, JsonParseException, message, (Location) location);

    private static ErrorDetail CreateDetail(string code, string format, string message)
        => new(code, string.Format(format, code, message));

    private static ErrorDetail CreateDetail(string code, string format, string message,
        Location location) => new(code, string.Format(format, location, code, message));
}