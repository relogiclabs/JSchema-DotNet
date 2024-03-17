using RelogicLabs.JSchema.Message;

namespace RelogicLabs.JSchema.Exceptions;

public class JsonSchemaException : ScriptRuntimeException
{
    public ErrorDetail Error { get; }
    public ExpectedDetail Expected { get; }
    public ActualDetail Actual { get; }

    public JsonSchemaException(ErrorDetail error, ExpectedDetail expected,
            ActualDetail actual, Exception? innerException = null)
        : base(error.Code, FormatMessage(error, expected, actual), innerException)
    {
        Error = error;
        Expected = expected;
        Actual = actual;
    }

    private static string FormatMessage(ErrorDetail error, ExpectedDetail expected,
            ActualDetail actual)
    {
        var context = expected.Context ?? actual.Context;
        return context.Runtime.MessageFormatter.Format(error, expected, actual);
    }
}