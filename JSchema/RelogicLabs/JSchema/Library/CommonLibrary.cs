using RelogicLabs.JSchema.Engine;
using RelogicLabs.JSchema.Exceptions;
using RelogicLabs.JSchema.Script;
using RelogicLabs.JSchema.Types;
using RelogicLabs.JSchema.Utilities;
using static RelogicLabs.JSchema.Engine.ScriptTreeHelper;
using static RelogicLabs.JSchema.Message.ErrorCode;
using static RelogicLabs.JSchema.Tree.IEFunction;

namespace RelogicLabs.JSchema.Library;

internal class CommonLibrary
{
    private const string Type_M0 = "type#0";
    private const string String_M0 = "string#0";
    private readonly Dictionary<string, MethodEvaluator> _methods;

    public static CommonLibrary Instance { get; } = new();
    protected virtual EType Type => EType.ANY;

    private protected CommonLibrary()
    {
        _methods = new Dictionary<string, MethodEvaluator>(20);
        AddMethod(Type_M0, TypeMethod);
        AddMethod(String_M0, StringMethod);
    }

    public MethodEvaluator GetMethod(string name, int argCount)
        => _methods.TryGetValue($"{name}#{argCount}")
            ?? _methods.TryGetValue($"{name}#{VariadicArity}")
            ?? throw FailOnMethodNotFound(name, argCount, Type);

    protected void AddMethod(string name, MethodEvaluator evaluator)
        => _methods.Add(name, evaluator);

    private static GString TypeMethod(IEValue self, List<IEValue> arguments, ScriptScope scope)
        => GString.From(self.Type.Name);

    private static GString StringMethod(IEValue self, List<IEValue> arguments, ScriptScope scope)
        => GString.From(Stringify(self));

    private static ScriptCommonException FailOnMethodNotFound(string name, int argCount, EType type)
        => new(MNVK01, $"Method '{name}' with {argCount} parameter(s) of {type} not found");

    protected static ScriptArgumentException FailOnInvalidArgumentType(string code, IEValue argument,
                string method, string parameter, IEValue self)
        => new(code, $"Invalid {argument.Type} argument for '{parameter}' parameter in '{
            method}' method of {self.Type}");
}