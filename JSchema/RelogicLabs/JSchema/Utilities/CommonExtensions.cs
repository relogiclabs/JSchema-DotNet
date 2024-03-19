using System.Reflection;

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
}