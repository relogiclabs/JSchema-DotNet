using RelogicLabs.JSchema.Engine;
using RelogicLabs.JSchema.Exceptions;
using RelogicLabs.JSchema.Script;
using RelogicLabs.JSchema.Types;
using RelogicLabs.JSchema.Utilities;
using static RelogicLabs.JSchema.Engine.ScriptTreeHelper;
using static RelogicLabs.JSchema.Message.OutlineFormatter;

namespace RelogicLabs.JSchema.Library;

internal sealed partial class ScriptLibrary
{
    private const string Print_F1 = "print#1";
    private const string Fail_F1 = "fail#1";
    private const string Fail_F2 = "fail#2";
    private const string Fail_F4 = "fail#4";
    private const string Expected_F1 = "expected#1";
    private const string Expected_F2 = "expected#2";
    private const string Actual_F1 = "actual#1";
    private const string Actual_F2 = "actual#2";
    private const string Ticks_F0 = "ticks#0";

    private const string Message_Id = "message";
    private const string Code_Id = "code";
    private const string Expected_Id = "expected";
    private const string Actual_Id = "actual";
    private const string Node_Id = "node";

    private static readonly ScriptLibrary Library = new();
    private readonly Dictionary<string, IRFunction> _functions;

    private ScriptLibrary()
    {
        _functions = new Dictionary<string, IRFunction>(10)
        {
            [Print_F1] = new LibraryFunction(PrintFunction, Message_Id),
            [Fail_F1] = new LibraryFunction(FailFunction1, Message_Id),
            [Fail_F2] = new LibraryFunction(FailFunction2, Code_Id, Message_Id),
            [Fail_F4] = new LibraryFunction(FailFunction4, Code_Id, Message_Id, Expected_Id, Actual_Id),
            [Expected_F1] = new LibraryFunction(ExpectedFunction1, Message_Id),
            [Expected_F2] = new LibraryFunction(ExpectedFunction2, Node_Id, Message_Id),
            [Actual_F1] = new LibraryFunction(ActualFunction1, Message_Id),
            [Actual_F2] = new LibraryFunction(ActualFunction2, Node_Id, Message_Id),
            [Ticks_F0] = new LibraryFunction(TicksFunction)
        };
    }

    public static IEValue? ResolveStatic(string name) => Library._functions.TryGetValue(name);

    private static long ToInteger(IEValue value, string parameter, string code)
        => value is not IEInteger i ? throw FailOnInvalidArgumentType(code,
            value, parameter) : i.Value;

    private static double ToNumber(IEValue value, string parameter, string code)
        => value is not IENumber n ? throw FailOnInvalidArgumentType(code,
            value, parameter) : n.ToDouble();

    private static string ToString(IEValue value, string parameter, string code)
        => value is not IEString s ? throw FailOnInvalidArgumentType(code,
            value, parameter) : s.Value;

    private static T Cast<T>(IEValue value, string parameter, string code) where T : class
        => value as T ?? throw FailOnInvalidArgumentType(code, value, parameter);

    private static ScriptArgumentException FailOnInvalidArgumentType(string code,
                IEValue argument, string parameter)
        => new(code, $"Invalid argument type {argument.Type} for parameter '{
            parameter}' of function '~%0'");

    private static T GetMember<T>(IEObject instance, string key, string parameter, string code)
    {
        var value = Dereference(instance.Get(key)!);
        if(value is not T t) throw FailOnInvalidArgumentValue(code, instance, parameter);
        return t;
    }

    private static ScriptArgumentException FailOnInvalidArgumentValue(string code,
                IEValue argument, string parameter)
        => new(code, $"Invalid argument value {
            CreateOutline(argument)} for parameter '{parameter}' of function '~%0'");

    private static void Fail(ScriptScope scope, Exception exception)
        => scope.GetRuntime().Exceptions.Fail(exception);
}