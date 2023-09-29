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

    public bool Even(JNumber target)
    {
        bool result = target % 2 == 0;
        if(!result) FailWith(new JsonSchemaException(
            new ErrorDetail(EVENFUNC01, "Number is not even"),
            new ExpectedDetail(target, "even number"),
            new ActualDetail(target, $"number {target} is odd")));
        return true;
    }

    public bool CanTest(JNumber target, JString str1, JBoolean bool1, params JNumber[] args)
        => true;
}