using System.Reflection;
using RelogicLabs.JsonSchema.Types;
using RelogicLabs.JsonSchema.Utilities;

namespace RelogicLabs.JsonSchema.Tree;

internal sealed record FunctionKey(string FunctionName, int ParameterCount)
{
    public const char EscapedPrefix = '_';

    public FunctionKey(JFunction function)
        : this(function.Name, function.Arguments.Count + 1) { }

    public FunctionKey(MethodInfo methodInfo, int parameterCount)
        : this(GetFunctionName(methodInfo), parameterCount) { }

    private static string GetFunctionName(MethodInfo methodInfo)
        => '@' + methodInfo.Name.TrimStart(EscapedPrefix).Uncapitalize();
}