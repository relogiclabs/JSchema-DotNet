using System.Reflection;
using RelogicLabs.JSchema.Exceptions;
using RelogicLabs.JSchema.Functions;
using RelogicLabs.JSchema.Message;
using RelogicLabs.JSchema.Nodes;
using RelogicLabs.JSchema.Utilities;
using static RelogicLabs.JSchema.Message.ContextDetail;
using static RelogicLabs.JSchema.Message.ErrorCode;
using static RelogicLabs.JSchema.Message.MessageFormatter;

namespace RelogicLabs.JSchema.Tree;

public sealed class FunctionRegistry
{
    private readonly HashSet<string> _imports = new();
    private readonly Dictionary<FunctionKey, List<IEFunction>> _functions = new();
    private readonly RuntimeContext _runtime;

    internal FunctionRegistry(RuntimeContext runtime) => _runtime = runtime;

    public JImport AddClass(JImport import)
    {
        AddClass(import.ClassName, import.Context);
        return import;
    }

    public void AddClass(string className, Context? context = null)
    {
        if(!_imports.Add(className)) throw new DuplicateImportException(FormatForSchema(
            CLAS01, $"Class already imported {className}", context));

        var providerClass = Type.GetType(className) ?? throw new ClassNotFoundException(
            FormatForSchema(CLAS02, $"Not found {className}", context));

        var baseClass = typeof(FunctionProvider);
        // if not FunctionProvider's subclass
        if(!baseClass.IsAssignableFrom(providerClass)) throw new InvalidImportException(
            FormatForSchema(CLAS03, $"{providerClass.FullName} needs to inherit {
                baseClass.FullName}", context));
        try
        {
            _functions.Merge(ExtractMethods(providerClass, CreateInstance(providerClass, context)));
        }
        catch(InvalidFunctionException ex)
        {
            throw new InvalidFunctionException(FormatForSchema(ex.Code, ex.Message, context), ex);
        }
    }

    private static Dictionary<FunctionKey, List<IEFunction>> ExtractMethods(Type providerClass,
                FunctionProvider instance)
    {
        var baseClass = typeof(FunctionProvider);
        var functions = new Dictionary<FunctionKey, List<IEFunction>>();
        foreach(var m in providerClass.GetMethods())
        {
            // Methods in ancestor class or in base/super class
            if(!baseClass.IsAssignableFrom(m.DeclaringType)
               || baseClass == m.DeclaringType) continue;
            ParameterInfo[] parameters = m.GetParameters();
            if(!IsValidReturnType(m.ReturnType)) throw new InvalidFunctionException(FUNC01,
                $"Function [{m.GetSignature()}] requires valid return type");
            if(parameters.Length == 0 || parameters[0].IsParams())
                throw new InvalidFunctionException(FUNC02,
                    $"Function [{m.GetSignature()}] requires target parameter");
            AddFunctionTo(new NativeFunction(m, parameters, instance), functions);
        }
        return functions;
    }

    private FunctionProvider CreateInstance(Type type, Context? context)
    {
        try
        {
            var instance = (FunctionProvider?) Activator.CreateInstance(type, _runtime);
            if(instance != null) return instance;
        }
        catch(TargetInvocationException ex) { throw FailOnCreateInstance(CLAS04, ex, type, context); }
        catch(MissingMethodException ex) { throw FailOnCreateInstance(CLAS05, ex, type, context); }
        catch(MethodAccessException ex) { throw FailOnCreateInstance(CLAS06, ex, type, context); }
        catch(MemberAccessException ex) { throw FailOnCreateInstance(CLAS07, ex, type, context); }
        catch(TypeLoadException ex) { throw FailOnCreateInstance(CLAS08, ex, type, context); }
        catch(FileNotFoundException ex) { throw FailOnCreateInstance(CLAS09, ex, type, context); }
        catch(NotSupportedException ex) { throw FailOnCreateInstance(CLAS10, ex, type, context); }
        catch(BadImageFormatException ex) { throw FailOnCreateInstance(CLAS11, ex, type, context); }
        catch(FileLoadException ex) { throw FailOnCreateInstance(CLAS12, ex, type, context); }
        catch(Exception ex) { throw FailOnCreateInstance(CLAS13, ex, type, context); }
        throw FailOnCreateInstance(CLAS14, null, type, context);
    }

    private static ClassInstantiationException FailOnCreateInstance(string code, Exception? ex,
                Type type, Context? context)
        => new(FormatForSchema(code, $"Fail to create instance of {type.FullName}", context), ex);

    private static bool IsValidReturnType(Type type)
    {
        if(type == typeof(bool)) return true;
        if(type == typeof(FutureFunction)) return true;
        return false;
    }

    public bool InvokeFunction(JFunction caller, JNode target)
    {
        foreach(var e in caller.Cache) if(e.IsTargetMatch(target))
            return ProcessResult(e.Invoke(caller, target));

        Type? mismatchTarget = null;
        foreach(var f in GetFunctions(caller))
        {
            var schemaArgs = f.Prebind(caller.Arguments);
            if(schemaArgs == null) continue;
            var targetType = f.TargetType;
            if(IsMatch(targetType, target))
            {
                var allArgs = AddTarget(schemaArgs, target).ToArray();
                var result = f.Invoke(caller, allArgs);
                caller.Cache.Add(f, allArgs);
                return ProcessResult(result);
            }
            mismatchTarget = targetType;
        }
        if(mismatchTarget != null)
            return Fail(new JsonSchemaException(new ErrorDetail(FUNC03,
                    $"Function {caller.GetOutline()} is incompatible with the target data type"),
                new ExpectedDetail(caller, $"applying to a supported data type such as {
                    GetTypeName(mismatchTarget)}"),
                new ActualDetail(target, $"applied to an unsupported data type {
                    GetTypeName(target.GetType())} of {target}")));
        return Fail(new FunctionNotFoundException(FormatForSchema(FUNC04,
            caller.GetOutline(), caller)));
    }

    private static IList<object> AddTarget(IList<object> arguments, JNode target)
    {
        arguments.Insert(0, GetDerived(target));
        return arguments;
    }

    private bool ProcessResult(object result)
    {
        return result is FutureFunction future
            ? _runtime.AddFuture(future)
            : (bool) result;
    }

    private List<IEFunction> GetFunctions(JFunction caller)
    {
        _functions.TryGetValue(new FunctionKey(caller), out var list);
        if(list == null) _functions.TryGetValue(new FunctionKey(caller.Name, -1), out list);
        if(list == null) throw new FunctionNotFoundException(FormatForSchema(FUNC05,
                $"Not found function {caller.GetOutline()}", caller));
        return list;
    }

    private static void AddFunctionTo(IEFunction function,
                Dictionary<FunctionKey, List<IEFunction>> functions)
    {
        var functionKey = new FunctionKey(function);
        functions.TryGetValue(functionKey, out var functionList);
        functionList ??= new List<IEFunction>();
        functionList.Add(function);
        functions[functionKey] = functionList;
    }

    internal void AddFunction(ScriptFunction function) => AddFunctionTo(function, _functions);
    private bool Fail(Exception exception) => _runtime.Exceptions.Fail(exception);

    internal static object GetDerived(object value)
        => value is IDerived derived ? derived.Derived ?? value : value;

    internal static bool IsMatch(Type type, object value)
        => type.IsInstanceOfType(GetDerived(value));
}