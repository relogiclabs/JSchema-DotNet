using Antlr4.Runtime.Tree;
using RelogicLabs.JSchema.Antlr;
using RelogicLabs.JSchema.Script;
using RelogicLabs.JSchema.Tree;
using RelogicLabs.JSchema.Types;
using static RelogicLabs.JSchema.Engine.ScriptErrorHelper;
using static RelogicLabs.JSchema.Script.IRFunction;
using static RelogicLabs.JSchema.Script.GFunction;
using static RelogicLabs.JSchema.Utilities.CommonUtilities;

namespace RelogicLabs.JSchema.Engine;

internal static class ScriptTreeHelper
{
    private const string TryofValue = "value";
    private const string TryofError = "error";

    public static bool AreEqual(IEValue? v1, IEValue? v2, RuntimeContext runtime)
    {
        v1 = Dereference(v1!);
        v2 = Dereference(v2!);
        if(v1 is IENumber n1 && v2 is IENumber n2)
            return runtime.AreEqual(n1.ToDouble(), n2.ToDouble());
        if(v1 is IEBoolean b1 && v2 is IEBoolean b2) return b1.Value == b2.Value;
        if(v1 is IEString s1 && v2 is IEString s2) return s1.Value == s2.Value;
        if(v1 is IENull && v2 is IENull) return true;
        if(v1 is IEUndefined && v2 is IEUndefined) return true;
        if(v1 is IEArray a1 && v2 is IEArray a2)
        {
            if(a1.Count != a2.Count) return false;
            for(var i = 0; i < a1.Count; i++)
                if(!AreEqual(a1.Get(i), a2.Get(i), runtime)) return false;
            return true;
        }
        if(v1 is IEObject o1 && v2 is IEObject o2)
        {
            if(o1.Count != o2.Count) return false;
            foreach(var k in o1.Keys)
                if(!AreEqual(o1.Get(k), o2.Get(k), runtime)) return false;
            return true;
        }
        if(v1 is GRange r1 && v2 is GRange r2) return r1.Equals(r2);
        return false;
    }

    public static IEValue Dereference(IEValue value)
    {
        while(value is GReference reference)
            value = reference.Value;
        return value;
    }

    public static void AreCompatible(IRFunction function, List<IEValue> arguments, string code)
    {
        var paramCount = function.Parameters.Length;
        if(function.Variadic)
        {
            if(arguments.Count < paramCount - 1) throw FailOnVariadicArgument(code);
            return;
        }
        if(arguments.Count != paramCount) throw FailOnFixedArgument(code);
    }

    public static string Stringify(object value)
        => value is IEString s ? s.Value : value.ToString()
                ?? throw new InvalidOperationException("Invalid runtime state");

    public static int GetFunctionMode(ITerminalNode constraint, ITerminalNode future,
        ITerminalNode subroutine) => GetFunctionMode(constraint)
                                     | GetFunctionMode(future)
                                     | GetFunctionMode(subroutine);

    private static int GetFunctionMode(ITerminalNode? node)
    {
        if(node == null) return 0;
        return node.Symbol.Type switch
        {
            SchemaLexer.G_CONSTRAINT => ConstraintMode,
            SchemaLexer.G_FUTURE => FutureMode,
            SchemaLexer.G_SUBROUTINE => SubroutineMode,
            _ => 0
        };
    }

    public static bool IsConstraint(int mode) => HasFlag(mode, ConstraintMode);
    public static string ToConstraintName(string functionName) => ConstraintPrefix + functionName;

    public static string FormatFunctionName(string baseName, GParameter[] parameters)
        => HasVariadic(parameters) ? baseName + "#..." : baseName + "#" + parameters.Length;

    public static GParameter[] ToParameters(IList<ITerminalNode> identifiers, ITerminalNode? ellipsis)
    {
        var group = identifiers.GroupBy(static i => i.GetText())
            .FirstOrDefault(static g => g.Count() > 1);
        var duplicate = group?.First();
        if(duplicate != null) throw FailOnDuplicateParameterName(duplicate);
        var parameters = identifiers.Select(static i => i.GetText()).ToList();
        if(ellipsis != null) UpdateLast(parameters, ellipsis.GetText());
        return parameters.Select(static p => new GParameter(p)).ToArray();
    }

    public static IENumber Increment(IENumber number)
        => number is IEInteger i ? GInteger.Of(i.Value + 1) : GDouble.Of(number.ToDouble() + 1);

    public static IENumber Decrement(IENumber number)
        => number is IEInteger i ? GInteger.Of(i.Value - 1) : GDouble.Of(number.ToDouble() - 1);

    public static GObject CreateTryofMonad(IEValue value, IEValue error)
    {
        var result = new GObject(2);
        result.Set(TryofValue, value);
        result.Set(TryofError, error);
        return result;
    }

    private static void UpdateLast(IList<string> list, string suffix) => list[^1] += suffix;
}