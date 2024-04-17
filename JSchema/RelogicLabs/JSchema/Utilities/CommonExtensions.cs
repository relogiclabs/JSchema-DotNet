using System.Reflection;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace RelogicLabs.JSchema.Utilities;

internal static class CommonExtensions
{
    internal static bool IsParams(this ParameterInfo parameter)
        => parameter.IsDefined(typeof(ParamArrayAttribute), false);

    internal static string GetSignature(this MethodInfo methodInfo)
    {
        var typeName = methodInfo.DeclaringType?.FullName!;
        var methodName = methodInfo.Name;
        var parameters = methodInfo.GetParameters()
            .Select(static p => $"{p.ParameterType.Name} {p.Name}")
            .JoinWith(", ", "(", ")");
        var returnType = methodInfo.ReturnType.Name;
        return $"{returnType} {typeName}.{methodName}{parameters}";
    }

    internal static ITerminalNode GetToken(this ParserRuleContext context, int type)
        => context.GetToken(type, 0);

    internal static bool HasToken(this ParserRuleContext context, int type)
        => context.GetToken(type, 0) != null;
}