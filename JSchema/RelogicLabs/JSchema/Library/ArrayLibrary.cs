using RelogicLabs.JSchema.Engine;
using RelogicLabs.JSchema.Script;
using RelogicLabs.JSchema.Types;
using static RelogicLabs.JSchema.Engine.ScriptTreeHelper;
using static RelogicLabs.JSchema.Message.ErrorCode;
using static RelogicLabs.JSchema.Types.IEUndefined;

namespace RelogicLabs.JSchema.Library;

internal sealed class ArrayLibrary : CommonLibrary
{
    private const string Length_M0 = "length#0";
    private const string Find_Bn = "find";
    private const string Find_M1 = "find#1";
    private const string Find_M2 = "find#2";
    private const string Fill_Bn = "fill";
    private const string Fill_M2 = "fill#2";
    private const string Copy_M0 = "copy#0";

    private const string Start_Id = "start";
    private const string Length_Id = "length";

    public new static ArrayLibrary Instance { get; } = new();
    protected override EType Type => EType.ARRAY;

    private ArrayLibrary()
    {
        AddMethod(Length_M0, LengthMethod);
        AddMethod(Find_M1, FindMethod1);
        AddMethod(Find_M2, FindMethod2);
        AddMethod(Fill_M2, FillMethod);
        AddMethod(Copy_M0, CopyMethod);
    }

    private static GInteger LengthMethod(IEValue self, List<IEValue> arguments, ScriptScope scope)
        => GInteger.From(((IEArray) self).Count);

    private static IEValue FindMethod1(IEValue self, List<IEValue> arguments, ScriptScope scope)
    {
        var runtime = scope.GetRuntime();
        var array = (IEArray) self;
        var value = arguments[0];
        for(var i = 0; i < array.Count; i++)
            if(AreEqual(array.Get(i), value, runtime)) return GInteger.From(i);
        return UNDEFINED;
    }

    private static IEValue FindMethod2(IEValue self, List<IEValue> arguments, ScriptScope scope)
    {
        var runtime = scope.GetRuntime();
        var array = (IEArray) self;
        var value = arguments[0];
        var start = arguments[1] is IEInteger s ? (int) s.Value
            : throw FailOnInvalidArgumentType(AFND01, arguments[1], Find_Bn, Start_Id, self);
        for(var i = start; i < array.Count; i++)
            if(AreEqual(array.Get(i), value, runtime)) return GInteger.From(i);
        return UNDEFINED;
    }

    private static IEValue FillMethod(IEValue self, List<IEValue> arguments, ScriptScope scope)
    {
        var length = arguments[1] is IEInteger l ? (int) l.Value
            : throw FailOnInvalidArgumentType(FILL01, arguments[1], Fill_Bn, Length_Id, self);
        return GArray.FilledFrom(arguments[0], length);
    }

    private static IEValue CopyMethod(IEValue self, List<IEValue> arguments, ScriptScope scope)
        => new GArray(((IEArray) self).Values);
}