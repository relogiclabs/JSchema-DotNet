using System.Text;
using Antlr4.Runtime;
using RelogicLabs.JSchema.Nodes;
using RelogicLabs.JSchema.Tree;
using RelogicLabs.JSchema.Utilities;
using static System.Environment;

namespace RelogicLabs.JSchema.Message;

// Exception Message Formatter
public abstract class MessageFormatter
{
    internal const string ErrorPointer = "<|>";

    private const string SchemaBasicFormat = "Schema Input [{0}]: {1}";
    private const string JsonBasicFormat = "Json Input [{0}]: {1}";
    private const string SchemaDetailFormat = "Schema (Line: {0}) [{1}]: {2}";
    private const string JsonDetailFormat = "Json (Line: {0}) [{1}]: {2}";

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

    private MessageFormatter(string summary, string expected, string actual)
    {
        Summary = summary;
        Expected = expected;
        Actual = actual;
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
        => location == null
            ? CreateError(code, SchemaBasicFormat, message)
            : CreateError(code, SchemaDetailFormat, message, (Location) location);

    internal static ErrorDetail FormatForSchema(string code, string message, IToken? token)
        => token == null
            ? CreateError(code, SchemaBasicFormat, message)
            : CreateError(code, SchemaDetailFormat, message, token);

    internal static ErrorDetail FormatForJson(string code, string message, JNode? node)
        => FormatForJson(code, message, node?.Context);

    internal static ErrorDetail FormatForJson(string code, string message, Context? context)
        => FormatForJson(code, message, context?.GetLocation());

    internal static ErrorDetail FormatForJson(string code, string message, Location? location)
        => location == null
            ? CreateError(code, JsonBasicFormat, message)
            : CreateError(code, JsonDetailFormat, message, (Location) location);

    private static ErrorDetail CreateError(string code, string format, string message)
        => new(code, string.Format(format, code, message));

    private static ErrorDetail CreateError(string code, string format, string message,
        Location location) => new(code, string.Format(format, location, code, message));

    private static ErrorDetail CreateError(string code, string format, string message,
        IToken token) => new(code, string.Format(format, $"{token.Line}:{token.Column}",
        code, message));
}