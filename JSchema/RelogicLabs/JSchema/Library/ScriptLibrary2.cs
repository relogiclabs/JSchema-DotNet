using RelogicLabs.JSchema.Engine;
using RelogicLabs.JSchema.Exceptions;
using RelogicLabs.JSchema.Message;
using RelogicLabs.JSchema.Nodes;
using RelogicLabs.JSchema.Script;
using RelogicLabs.JSchema.Types;
using static RelogicLabs.JSchema.Engine.ScriptTreeHelper;
using static RelogicLabs.JSchema.Message.ErrorCode;
using static RelogicLabs.JSchema.Message.MessageFormatter;
using static RelogicLabs.JSchema.Script.GBoolean;
using static RelogicLabs.JSchema.Tree.ScriptFunction;
using static RelogicLabs.JSchema.Types.IEValue;

namespace RelogicLabs.JSchema.Library;

internal sealed partial class ScriptLibrary
{
    private static IEValue PrintFunction(ScriptScope scope, List<IEValue> arguments)
    {
        Console.WriteLine(Stringify(arguments[0]));
        return VOID;
    }

    private static IEValue FailFunction1(ScriptScope scope, List<IEValue> arguments)
    {
        var caller = scope.Resolve(CALLER_HVAR);
        Fail(scope, new ScriptInitiatedException(FormatForSchema(FAIL01,
            ToString(arguments[0], Message_Id, FAIL02), (JNode?) caller)));
        return FALSE;
    }

    private static IEValue FailFunction2(ScriptScope scope, List<IEValue> arguments)
    {
        var caller = scope.Resolve(CALLER_HVAR);
        Fail(scope, new ScriptInitiatedException(FormatForSchema(
            ToString(arguments[0], Code_Id, FAIL03),
            ToString(arguments[1], Message_Id, FAIL04), (JNode?) caller)));
        return FALSE;
    }

    private static IEValue FailFunction4(ScriptScope scope, List<IEValue> arguments)
    {
        var expected = Cast<IEObject>(arguments[2], Expected_Id, FAIL05);
        var actual = Cast<IEObject>(arguments[3], Actual_Id, FAIL06);
        Fail(scope, new JsonSchemaException(new ErrorDetail(
                ToString(arguments[0], Code_Id, FAIL07),
                ToString(arguments[1], Message_Id, FAIL08)),
            new ExpectedDetail(GetMember<JNode>(expected, Node_Id, Expected_Id, FAIL09),
                GetMember<IEString>(expected, Message_Id, Expected_Id, FAIL10).Value),
            new ActualDetail(GetMember<JNode>(actual, Node_Id, Actual_Id, FAIL11),
                GetMember<IEString>(actual, Message_Id, Actual_Id, FAIL12).Value)));
        return FALSE;
    }

    private static IEValue ExpectedFunction1(ScriptScope scope, List<IEValue> arguments)
    {
        var result = new GObject(2);
        result.Put(Node_Id, scope.Resolve(CALLER_HVAR) ?? VOID);
        result.Put(Message_Id, Cast<IEString>(arguments[0], Message_Id, EXPC01));
        return result;
    }

    private static IEValue ExpectedFunction2(ScriptScope scope, List<IEValue> arguments)
    {
        var result = new GObject(2);
        result.Put(Node_Id, Cast<JNode>(arguments[0], Node_Id, EXPC02));
        result.Put(Message_Id, Cast<IEString>(arguments[1], Message_Id, EXPC03));
        return result;
    }

    private static IEValue ActualFunction1(ScriptScope scope, List<IEValue> arguments)
    {
        var result = new GObject(2);
        result.Put(Node_Id, scope.Resolve(TARGET_HVAR) ?? VOID);
        result.Put(Message_Id, Cast<IEString>(arguments[0], Message_Id, ACTL01));
        return result;
    }

    private static IEValue ActualFunction2(ScriptScope scope, List<IEValue> arguments)
    {
        var result = new GObject(2);
        result.Put(Node_Id, Cast<JNode>(arguments[0], Node_Id, ACTL02));
        result.Put(Message_Id, Cast<IEString>(arguments[1], Message_Id, ACTL03));
        return result;
    }

    private static IEValue TicksFunction(ScriptScope scope, List<IEValue> arguments)
        => GInteger.From(Environment.TickCount64);
}