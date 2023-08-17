using System.Reflection;
using RelogicLabs.JsonSchema.Functions;
using RelogicLabs.JsonSchema.Types;
using RelogicLabs.JsonSchema.Utilities;

namespace RelogicLabs.JsonSchema.Tree;

internal class MethodPointer
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

    public bool Invoke(JFunction function, List<object> arguments)
    {
        Instance.Function = function;
        var result = Method.Invoke(Instance, arguments.ToArray());
        if(result is not bool _result) throw new InvalidOperationException();
        return _result;
    }
}