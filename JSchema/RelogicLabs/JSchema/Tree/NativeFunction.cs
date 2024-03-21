using System.Reflection;
using RelogicLabs.JSchema.Exceptions;
using RelogicLabs.JSchema.Functions;
using RelogicLabs.JSchema.Nodes;
using RelogicLabs.JSchema.Types;
using RelogicLabs.JSchema.Utilities;
using static RelogicLabs.JSchema.Message.ErrorCode;
using static RelogicLabs.JSchema.Tree.FunctionRegistry;

namespace RelogicLabs.JSchema.Tree;

internal sealed class NativeFunction : IEFunction
{
    public MethodInfo Method { get; }
    public ParameterInfo[] Parameters { get; }
    public FunctionProvider Instance { get; }
    public int Arity { get; }
    public Type TargetType { get; }
    public string Name => Method.Name;

    public NativeFunction(MethodInfo method, ParameterInfo[] parameters,
                FunctionProvider instance)
    {
        Method = method;
        Parameters = parameters;
        Instance = instance;
        Arity = parameters[^1].IsParams() ? -1 : parameters.Length;
        TargetType = Parameters[0].ParameterType;
    }

    public IList<object>? Prebind<T>(IList<T> arguments) where T : IEValue
    {
        var count = Parameters.Length;
        if(Arity == -1 && arguments.Count < count - 2) return null;
        var result = new List<object>(count);
        for(var i = 1; i < count; i++)
        {
            if(Parameters[i].IsParams())
            {
                var args = ProcessParams(Parameters[i], arguments.GetRange(i - 1));
                if(args == null) return null;
                result.Add(args);
                break;
            }
            if(!IsMatch(Parameters[i], arguments[i - 1])) return null;
            result.Add(arguments[i - 1]);
        }
        return result;
    }

    public object Invoke(JFunction caller, object[] arguments)
    {
        Instance.Caller = caller;
        var result = Method.Invoke(Instance, arguments);
        if(result == null) throw new InvalidFunctionException(FUNC09,
            $"Function {Method.Name} must not return null");
        return result;
    }

    private static bool IsMatch(ParameterInfo parameter, IEValue argument)
        => parameter.ParameterType.IsInstanceOfType(GetDerived(argument));

    private static Array? ProcessParams<T>(ParameterInfo parameter, IList<T> arguments)
    {
        var elementType = parameter.ParameterType.GetElementType();
        if(elementType == null) throw new InvalidOperationException("Invalid function parameter");
        var result = Array.CreateInstance(elementType, arguments.Count);
        for(var i = 0; i < arguments.Count; i++)
        {
            var arg = arguments[i];
            if(!elementType.IsInstanceOfType(arg)) return null;
            result.SetValue(arg, i);
        }
        return result;
    }
}