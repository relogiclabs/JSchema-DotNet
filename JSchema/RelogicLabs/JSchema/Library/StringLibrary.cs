using RelogicLabs.JSchema.Engine;
using RelogicLabs.JSchema.Script;
using RelogicLabs.JSchema.Types;
using static RelogicLabs.JSchema.Message.ErrorCode;
using static RelogicLabs.JSchema.Types.IEUndefined;

namespace RelogicLabs.JSchema.Library;

internal sealed class StringLibrary : CommonLibrary
{
    private const string Length_M0 = "length#0";
    private const string Find_Bn = "find";
    private const string Find_M1 = "find#1";
    private const string Find_M2 = "find#2";
    private const string Copy_M0 = "copy#0";

    private const string Value_Id = "value";
    private const string Start_Id = "start";

    public new static StringLibrary Instance { get; } = new();
    protected override EType Type => EType.STRING;

    private StringLibrary()
    {
        AddMethod(Length_M0, LengthMethod);
        AddMethod(Find_M1, FindMethod1);
        AddMethod(Find_M2, FindMethod2);
        AddMethod(Copy_M0, CopyMethod);
    }

    private static GInteger LengthMethod(IEValue self, List<IEValue> arguments, ScriptScope scope)
        => GInteger.From(((IEString) self).Length);

    private static IEValue FindMethod1(IEValue self, List<IEValue> arguments, ScriptScope scope)
    {
        var value = arguments[0] is IEString v ? v.Value
            : throw FailOnInvalidArgumentType(SFND01, arguments[0], Find_Bn, Value_Id, self);
        var index = ((IEString) self).Value.IndexOf(value, StringComparison.Ordinal);
        return index == -1 ? UNDEFINED : GInteger.From(index);
    }

    private static IEValue FindMethod2(IEValue self, List<IEValue> arguments, ScriptScope scope)
    {
        var value = arguments[0] is IEString v ? v.Value
            : throw FailOnInvalidArgumentType(SFND02, arguments[0], Find_Bn, Value_Id, self);
        var start = arguments[1] is IEInteger s ? (int) s.Value
            : throw FailOnInvalidArgumentType(SFND03, arguments[1], Find_Bn, Start_Id, self);
        var index = ((IEString) self).Value.IndexOf(value, start, StringComparison.Ordinal);
        return index == -1 ? UNDEFINED : GInteger.From(index);
    }

    private static IEValue CopyMethod(IEValue self, List<IEValue> arguments, ScriptScope scope)
        => GString.From(((IEString) self).Value);
}