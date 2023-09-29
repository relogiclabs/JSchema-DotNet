using System.Reflection;

namespace RelogicLabs.JsonSchema.Utilities;

internal static class CommonExtension
{
    public static string GetSignature(this MethodInfo methodInfo)
    {
        string typeName = methodInfo.DeclaringType?.FullName!;
        string methodName = methodInfo.Name;
        string parameters = string.Join(", ", methodInfo.GetParameters()
            .Select(p => p.ParameterType.Name));
        string returnType = methodInfo.ReturnType.Name;
        string signature = $"{returnType} {typeName}.{methodName}({parameters})";
        return signature;
    }
}