using RelogicLabs.JsonSchema.Message;
using RelogicLabs.JsonSchema.Tree;

namespace RelogicLabs.JsonSchema.Exceptions;

public class JsonSchemaException : CommonException
{
    public ExpectedDetail Expected { get; }
    public ActualDetail Actual { get; }
    
    public JsonSchemaException(ErrorDetail error, ExpectedDetail expected, ActualDetail actual) 
        : base(error.Code, Format(error, expected, actual))
    {
        Expected = expected;
        Actual = actual;
    }
    
    public JsonSchemaException(ErrorDetail error, ExpectedDetail expected, 
        ActualDetail actual, Exception innerException) 
        : base(error.Code, Format(error, expected, actual), innerException)
    {
        Expected = expected;
        Actual = actual;
    }

    private static string Format(ErrorDetail error, ExpectedDetail expected, 
        ActualDetail actual)
    {
        Context context = expected.Context ?? actual.Context;
        return context.MessageFormatter.Format(error, expected, actual);
    }
}