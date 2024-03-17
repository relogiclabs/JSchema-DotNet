using System.Reflection;
using RelogicLabs.JsonSchema.Functions;
using RelogicLabs.JsonSchema.Types;
using RelogicLabs.JsonSchema.Utilities;

namespace RelogicLabs.JsonSchema.Tree;

internal sealed class MethodPointer
{
    public FunctionBase Instance { get; }
    public MethodInfo Method { get; }
    public IList<ParameterInfo> Parameters { get;}

    internal MethodPointer(FunctionBase instance, MethodInfo method,
        IEnumerable<ParameterInfo> parameters)
    {
        Instance = instance;
        Method = method;
        Parameters = parameters.ToReadOnlyList();
    }

    public object Invoke(JFunction function, object?[] arguments)
    {
        Instance.Function = function;
        var result = Method.Invoke(Instance, arguments);
        if(result is null) throw new InvalidOperationException();
        return result;
    }
}