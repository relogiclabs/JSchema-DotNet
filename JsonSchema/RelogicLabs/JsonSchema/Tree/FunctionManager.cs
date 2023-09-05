using System.Reflection;
using RelogicLabs.JsonSchema.Exceptions;
using RelogicLabs.JsonSchema.Functions;
using RelogicLabs.JsonSchema.Message;
using RelogicLabs.JsonSchema.Types;
using RelogicLabs.JsonSchema.Utilities;
using static RelogicLabs.JsonSchema.Message.ErrorCode;

namespace RelogicLabs.JsonSchema.Tree;

internal class FunctionManager
{
    private readonly HashSet<string> _includes = new();
    private readonly Dictionary<FunctionKey, List<MethodPointer>> _functions = new();
    private readonly RuntimeContext _runtime;

    public FunctionManager(RuntimeContext runtime) => _runtime = runtime;

    public void AddFunctions(string className, Context? context = null)
    {
        if(!_includes.Contains(className)) _includes.Add(className);
        else throw new DuplicateIncludeException(MessageFormatter.FormatForSchema(
            CLAS01, $"Class already included [{className}]", context));
        
        var subclass = Type.GetType(className) ?? throw new ClassNotFoundException(
            MessageFormatter.FormatForSchema(CLAS02, $"Not found {className}", context));
        
        Type baseclass = typeof(FunctionBase);
        // if not FunctionBase's subclass
        if(!baseclass.IsAssignableFrom(subclass)) 
            throw new InvalidIncludeException(MessageFormatter
                .FormatForSchema(CLAS03, $"{subclass.FullName} needs to inherit {
                    baseclass.FullName}", context));
        
        FunctionBase instance = CreateInstance(subclass, context);
        try
        {
            var functions = ExtractMethods(subclass, instance);
            _functions.Merge(functions);
        }
        catch(InvalidFunctionException ex)
        {
            throw new InvalidFunctionException(MessageFormatter
                .FormatForSchema(ex.ErrorCode, ex.Message, context));
        }
    }

    private FunctionBase CreateInstance(Type type, Context? context)
    {
        try
        {
            var instance = (FunctionBase?) Activator.CreateInstance(type, _runtime);
            if(instance != null) return instance;
        }
        catch(Exception ex)
        {
            throw CreateException(CLAS04, ex);
        }
        throw CreateException(CLAS05, null);
        
        Exception CreateException(string code, Exception? ex)
        {
            return new ClassInstantiationException(MessageFormatter.FormatForSchema(
                code, $"Fail to create instance of {type.FullName}", context), ex);
        }
    }

    private Dictionary<FunctionKey, List<MethodPointer>> ExtractMethods(
        Type subclass, FunctionBase instance)
    {
        var baseclass = typeof(FunctionBase);
        Dictionary<FunctionKey, List<MethodPointer>> functions = new();
        foreach(var m in subclass.GetMethods())
        {
            if(!baseclass.IsAssignableFrom(m.DeclaringType)) continue;
            if(baseclass == m.DeclaringType) continue;
            ParameterInfo[] parameters = m.GetParameters();
            if(m.ReturnType != typeof(bool)) throw new InvalidFunctionException(FUNC01,
                $"Function [{m.GetMethodHeader()}] requires return type boolean");
            if(parameters.Length < 1) throw new InvalidFunctionException(FUNC02,
                $"Function [{m.GetMethodHeader()}] requires minimum one parameter");
            var key = new FunctionKey(m, GetParameterCount(parameters));
            var value = new MethodPointer(instance, m, parameters);
            functions.TryGetValue(key, out var valueList);
            valueList ??= new List<MethodPointer>();
            valueList.Add(value);
            functions[key] = valueList;
        }
        return functions;
    }
    
    private int GetParameterCount(ICollection<ParameterInfo> parameters)
    {
        foreach(var p in parameters) if(IsParams(p)) return -1;
        return parameters.Count;
    }

    private static bool IsMatch(ParameterInfo parameter, JNode argument) 
        => parameter.ParameterType.IsInstanceOfType(argument);
    
    private static bool IsParams(ParameterInfo parameter) 
        => parameter.IsDefined(typeof(ParamArrayAttribute), false);

    private static object? ProcessParams(ParameterInfo parameter, IList<JNode> arguments)
    {
        Type? elementType = parameter.ParameterType.GetElementType();
        if(elementType == null) throw new InvalidOperationException();
        Array _arguments = Array.CreateInstance(elementType, arguments.Count);
        for(var i = 0; i < arguments.Count; i++)
        {
            var arg = arguments[i];
            if(!elementType.IsInstanceOfType(arg)) return null;
            _arguments.SetValue(arg, i);
        }
        return _arguments;
    }
    
    public bool InvokeFunction(JFunction function, JNode target)
    {
        var methods = GetMethods(function);
        string? mismatchMessage = null;
        foreach(var method in methods)
        {
            var _parameters = method.Parameters;
            var _arguments = function.Arguments;
            var schemaArgs = MatchSchemaArguments(_parameters, _arguments);
            if(schemaArgs == null) continue;
            if(!IsMatch(_parameters[0], target))
            {
                mismatchMessage = $"Function {function.ToOutline()} is applicable on {
                    _parameters[0].ParameterType.Name} but applied on {
                        target.GetType().Name} of {target}";
                continue;
            }
            return method.Invoke(function, JoinArguments(target, schemaArgs));
        }
        if(mismatchMessage != null) return FailWith(new FunctionMismatchException(
            MessageFormatter.FormatForSchema(FUNC03, mismatchMessage, function.Context)));
        return FailWith(new FunctionNotFoundException(MessageFormatter
            .FormatForSchema(FUNC04, function.ToOutline(), function.Context)));
    }

    private List<object> JoinArguments(JNode node, IList<object> arguments)
    {
        List<object> _arguments = new(1 + arguments.Count) { node };
        _arguments.AddRange(arguments);
        return _arguments;
    }

    private List<MethodPointer> GetMethods(JFunction function)
    {
        _functions.TryGetValue(new FunctionKey(function), out var methodPointers);
        if(methodPointers == null)
            _functions.TryGetValue(new FunctionKey(
                function.Name, -1), out methodPointers);
        if(methodPointers == null)
            throw new FunctionNotFoundException(MessageFormatter
                .FormatForSchema(FUNC05, $"Not found {function.ToOutline()}", function.Context));
        return methodPointers;
    }

    private List<object>? MatchSchemaArguments(IList<ParameterInfo> parameters, IList<JNode> arguments)
    {
        List<object> _arguments = new();
        for(int i = 1; i < parameters.Count; i++)
        {
            if(IsParams(parameters[i]))
            {
                IList<JNode> _rest = arguments.GetRange(i - 1, arguments.Count - i + 1);
                var args = ProcessParams(parameters[i], _rest);
                if(args == null) return null;
                _arguments.AddRange(arguments.GetRange(0, i - 1));
                _arguments.Add(args);
                break;
            }
            if(!IsMatch(parameters[i], arguments[i - 1])) return null;
            _arguments.Add(arguments[i - 1]);
        }
        return _arguments;
    }
    
    private bool FailWith(Exception exception)
    {
        if(_runtime.ThrowException) throw exception;
        _runtime.ErrorQueue.Enqueue(exception);
        return false;
    }
}