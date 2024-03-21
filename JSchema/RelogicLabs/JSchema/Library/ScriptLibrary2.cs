using RelogicLabs.JSchema.Engine;
using RelogicLabs.JSchema.Exceptions;
using RelogicLabs.JSchema.Nodes;
using RelogicLabs.JSchema.Types;
using static RelogicLabs.JSchema.Engine.ScriptTreeHelper;
using static RelogicLabs.JSchema.Message.OutlineFormatter;
using static RelogicLabs.JSchema.Types.IEValue;

namespace RelogicLabs.JSchema.Library;

internal sealed partial class ScriptLibrary
{
    private static readonly ScriptLibrary Library = new();
    private readonly Dictionary<string, IEValue> _symbols;

    private ScriptLibrary()
    {
        _symbols = new Dictionary<string, IEValue>(30);
        ScriptPrintFunction();
        ScriptTypeFunction();
        ScriptSizeFunction();
        ScriptStringifyFunction();
        ScriptFindFunction1();
        ScriptFindFunction2();
        ScriptRegularFunction();
        ScriptFailFunction1();
        ScriptFailFunction2();
        ScriptFailFunction3();
        ScriptExpectedFunction1();
        ScriptExpectedFunction2();
        ScriptActualFunction1();
        ScriptActualFunction2();
        ScriptCopyFunction();
        ScriptFillFunction();
        ScriptCeilFunction();
        ScriptFloorFunction();
        ScriptModFunction();
        ScriptPowFunction();
        ScriptLogFunction();
        ScriptTicksFunction();
    }

    public static IEValue ResolveStatic(string name)
    {
        Library._symbols.TryGetValue(name, out var value);
        return value ?? VOID;
    }

    private static long GetInteger(IEValue value, string parameter, string code)
        => value is not IEInteger i ? throw FailOnInvalidArgumentType(code,
            parameter, value) : i.Value;

    private static double GetNumber(IEValue value, string parameter, string code)
        => value is not IENumber n ? throw FailOnInvalidArgumentType(code,
            parameter, value) : n.ToDouble();

    private static string GetString(IEValue value, string parameter, string code)
        => value is not IEString s ? throw FailOnInvalidArgumentType(code,
            parameter, value) : s.Value;

    private static T Cast<T>(IEValue value, string parameter, string code) where T : class
        => value as T ?? throw FailOnInvalidArgumentType(code, parameter, value);

    private static ScriptArgumentException FailOnInvalidArgumentType(string code,
                string parameter, IEValue value)
        => new(code, $"Invalid argument type {value.Type} for parameter '{
            parameter}' of function '~%0'");

    private static T GetValue<T>(IEObject source, string key, string parameter, string code)
    {
        var value = Dereference(source.Get(key)!);
        if(value is not T t) throw FailOnInvalidArgumentValue(code, parameter, source);
        return t;
    }

    private static ScriptArgumentException FailOnInvalidArgumentValue(string code,
                string parameter, IEValue value)
        => new(code, $"Invalid argument value {
            CreateOutline(value)} for parameter '{parameter}' of function '~%0'");

    private static void Fail(ScopeContext scope, Exception exception)
        => scope.GetRuntime().Exceptions.Fail(exception);
}