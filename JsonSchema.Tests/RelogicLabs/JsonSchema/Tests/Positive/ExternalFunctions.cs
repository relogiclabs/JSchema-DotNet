using RelogicLabs.JsonSchema.Exceptions;
using RelogicLabs.JsonSchema.Functions;
using RelogicLabs.JsonSchema.Message;
using RelogicLabs.JsonSchema.Tree;
using RelogicLabs.JsonSchema.Types;

namespace RelogicLabs.JsonSchema.Tests.Positive;

public class ExternalFunctions : FunctionBase
{
    public const string EVENFUNC01 = "EVENFUNC01";
    
    public ExternalFunctions(RuntimeContext runtime) : base(runtime) { }

    public bool Even(JNumber source)
    {
        bool result = source % 2 == 0;
        if(!result) FailWith(new JsonSchemaException(
            new ErrorDetail(EVENFUNC01, "Number is not even"),
            new ExpectedDetail(source, "even number"),
            new ActualDetail(source, $"number {source} is odd")));
        return true;
    }
}