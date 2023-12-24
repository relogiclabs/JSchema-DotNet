using RelogicLabs.JsonSchema.Exceptions;
using RelogicLabs.JsonSchema.Functions;
using RelogicLabs.JsonSchema.Message;
using RelogicLabs.JsonSchema.Tree;
using RelogicLabs.JsonSchema.Types;
using RelogicLabs.JsonSchema.Utilities;

// Functions for positive (valid) test cases
namespace RelogicLabs.JsonSchema.Tests.External;

public class ExternalFunctions : FunctionBase
{
    public const string EVENFUNC01 = "EVENFUNC01";
    public const string ERRACCESS01 = "ERRACCESS01";
    public const string CONDFUNC01 = "CONDFUNC01";
    public const string CONDFUNC02 = "CONDFUNC02";
    public const string SUMEQUAL01 = "SUMEQUAL01";
    public const string MINMAX01 = "MINMAX01";

    public ExternalFunctions(RuntimeContext runtime) : base(runtime) { }

    public bool Even(JNumber target)
    {
        bool result = target % 2 == 0;
        if(!result) return FailWith(new JsonSchemaException(
            new ErrorDetail(EVENFUNC01, "Number is not even"),
            new ExpectedDetail(target, "even number"),
            new ActualDetail(target, $"number {target} is odd")));
        return true;
    }

    public bool CanTest(JNumber target, JString str1, JBoolean bool1, params JNumber[] args)
    {
        Console.WriteLine("Target: " + target);
        Console.WriteLine("String Parameter: " + str1);
        Console.WriteLine("Boolean Parameter: " + bool1);
        Console.WriteLine("Params Numbers: " + args.Join(",", "[", "]"));
        return true;
    }

    public bool CheckAccess(JInteger target, JReceiver userRole)
    {
        string role = userRole.GetValueNode<JString>();
        if(role == "user" && target > 5) return FailWith(new JsonSchemaException(
            new ErrorDetail(ERRACCESS01, "Data access incompatible with 'use' role"),
            new ExpectedDetail(target, "an access at most 5 for 'user' role"),
            new ActualDetail(target, $"found access {target} which is greater than 5")));
        return true;
    }

    public bool Condition(JInteger target, JReceiver receiver)
    {
        var threshold = receiver.GetValueNode<JInteger>();
        Console.WriteLine("Received threshold: " + threshold);
        bool result = threshold < target;
        if(!result) return FailWith(new JsonSchemaException(
            new ErrorDetail(CONDFUNC01, "Number does not satisfy the condition"),
            new ExpectedDetail(target, $"a number > {threshold} of '{receiver.Name}'"),
            new ActualDetail(target, $"found number {target} <= {threshold}")));
        return result;
    }

    public bool ConditionAll(JInteger target, JReceiver receiver)
    {
        var list = receiver.GetValueNodes<JInteger>();
        var values = list.Join(",", "[", "]");
        Console.WriteLine("Target: " + target);
        Console.WriteLine("Received integers: " + values);
        bool result = list.All(i => i < target);
        if(!result) return FailWith(new JsonSchemaException(
            new ErrorDetail(CONDFUNC02, "Number does not satisfy the condition"),
            new ExpectedDetail(target, $"a number > any of {values} of '{receiver.Name}'"),
            new ActualDetail(target, $"found number {target} <= some of {values}")));
        return true;
    }

    public FutureValidator SumEqual(JInteger target, JReceiver receiver)
    {
        return () =>
        {
            var values = receiver.GetValueNodes<JInteger>();
            var expression = values.Join("+");
            Console.WriteLine("Target: " + target);
            Console.WriteLine("Received values: " + expression);
            int result = values.Sum(i => (int) i);
            if(result != target)
                return FailWith(new JsonSchemaException(
                    new ErrorDetail(SUMEQUAL01, $"Number != sum of {expression} = {result}"),
                    new ExpectedDetail(target, $"a number = sum of numbers {result}"),
                    new ActualDetail(target, $"found number {target} != {result}")));
            return true;
        };
    }

    public FutureValidator Minmax(JInteger target, JReceiver min, JReceiver max)
    {
        return () =>
        {
            var intMin = min.GetValueNode<JInteger>();
            var intMax = max.GetValueNode<JInteger>();
            Console.WriteLine("Target: " + target);
            Console.WriteLine($"Received min: {intMin}, max: {intMax}");
            bool result = target >= intMin && target <= intMax;
            if(!result)
                return FailWith(new JsonSchemaException(
                    new ErrorDetail(MINMAX01, "Number is outside of range"),
                    new ExpectedDetail(target, $"a number in range [{intMin}, {intMax}]"),
                    new ActualDetail(target, $"found number {target} not in range")));
            return true;
        };
    }
}