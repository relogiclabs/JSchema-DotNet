using System.Reflection;
using RelogicLabs.JsonSchema.Types;
using RelogicLabs.JsonSchema.Utilities;

namespace RelogicLabs.JsonSchema.Tree;

internal record FunctionKey(string FunctionName, int ParameterCount)
{
    public FunctionKey(JFunction function) 
        : this(function.Name, function.Arguments.Count + 1) { }

    public FunctionKey(MethodInfo methodInfo, int parameterCount) 
        : this('@' + methodInfo.Name.ToLowerFirstLetter(), parameterCount) { }
}