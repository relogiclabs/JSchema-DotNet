using RelogicLabs.JsonSchema.Message;

namespace RelogicLabs.JsonSchema.Exceptions;

public class JsonSchemaException : CommonException
{
    public ErrorDetail Error { get; }
    public ExpectedDetail Expected { get; }
    public ActualDetail Actual { get; }

    public JsonSchemaException(ErrorDetail error, ExpectedDetail expected,
                    ActualDetail actual, Exception? innerException = null)
        : base(error.Code, Format(error, expected, actual), innerException)
    {
        Error = error;
        Expected = expected;
        Actual = actual;
    }

    private static string Format(ErrorDetail error, ExpectedDetail expected,
        ActualDetail actual)
    {
        var context = expected.Context ?? actual.Context;
        return context.Runtime.MessageFormatter.Format(error, expected, actual);
    }
}