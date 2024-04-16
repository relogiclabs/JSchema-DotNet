using RelogicLabs.JSchema.Engine;
using RelogicLabs.JSchema.Script;
using RelogicLabs.JSchema.Types;
using static RelogicLabs.JSchema.Message.ErrorCode;

namespace RelogicLabs.JSchema.Library;

internal sealed class NumberLibrary : CommonLibrary
{
    private const string Pow_Bn = "pow";
    private const string Pow_M1 = "pow#1";
    private const string Log_M0 = "log#0";
    private const string Ceil_M0 = "ceil#0";
    private const string Floor_M0 = "floor#0";
    private const string Copy_M0 = "copy#0";

    private const string Value_Id = "value";

    public new static NumberLibrary Instance { get; } = new();
    protected override EType Type => EType.NUMBER;

    private NumberLibrary()
    {
        AddMethod(Pow_M1, PowMethod);
        AddMethod(Log_M0, LogMethod);
        AddMethod(Ceil_M0, CeilMethod);
        AddMethod(Floor_M0, FloorMethod);
        AddMethod(Copy_M0, CopyMethod);
    }

    private static GDouble PowMethod(IEValue self, List<IEValue> arguments, ScriptScope scope)
    {
        var value = arguments[0] is IENumber v ? v.ToDouble()
            : throw FailOnInvalidArgumentType(POWR01, arguments[0], Pow_Bn, Value_Id, self);
        return GDouble.From(Math.Pow(((IENumber) self).ToDouble(), value));
    }

    private static GDouble LogMethod(IEValue self, List<IEValue> arguments, ScriptScope scope)
        => GDouble.From(Math.Log(((IENumber) self).ToDouble()));

    private static GInteger CeilMethod(IEValue self, List<IEValue> arguments, ScriptScope scope)
        => GInteger.From((long) Math.Ceiling(((IENumber) self).ToDouble()));

    private static GInteger FloorMethod(IEValue self, List<IEValue> arguments, ScriptScope scope)
        => GInteger.From((long) Math.Floor(((IENumber) self).ToDouble()));

    private static IEValue CopyMethod(IEValue self, List<IEValue> arguments, ScriptScope scope)
    {
        if(self is IEInteger i) return GInteger.From(i.Value);
        if(self is IENumber n) return GDouble.From(n.ToDouble());
        throw new InvalidOperationException("Invalid runtime state");
    }
}