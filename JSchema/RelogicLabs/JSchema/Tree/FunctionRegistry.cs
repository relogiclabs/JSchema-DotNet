using RelogicLabs.JSchema.Exceptions;
using RelogicLabs.JSchema.Functions;
using RelogicLabs.JSchema.Message;
using RelogicLabs.JSchema.Nodes;
using static RelogicLabs.JSchema.Message.ContextDetail;
using static RelogicLabs.JSchema.Message.ErrorCode;
using static RelogicLabs.JSchema.Message.MessageFormatter;

namespace RelogicLabs.JSchema.Tree;

public sealed class FunctionRegistry
{
    private readonly HashSet<string> _imports = new();
    private readonly FunctionMap _functions = new();
    private readonly RuntimeContext _runtime;

    internal FunctionRegistry(RuntimeContext runtime) => _runtime = runtime;

    public JImport AddImport(JImport import)
    {
        AddClass(import.ClassName, import.Context);
        return import;
    }

    private void AddClass(string className, Context context)
    {
        if(!_imports.Add(className)) throw new DuplicateImportException(FormatForSchema(
            CLAS01, $"Class already imported {className}", context));

        var providerImpl = Type.GetType(className) ?? throw new ClassNotFoundException(
            FormatForSchema(CLAS02, $"Not found {className}", context));

        var providerBase = typeof(FunctionProvider);
        if(!providerBase.IsAssignableFrom(providerImpl)) throw new InvalidImportException(
            FormatForSchema(CLAS03, $"{providerImpl.FullName} needs to inherit {
                providerBase.FullName}", context));
        _functions.MergeWith(FunctionLoader.Load(providerImpl, context));
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
        var key1 = FunctionId.Generate(caller);
        var key2 = FunctionId.Generate(caller, true);
        return _functions.TryGetValue(key1)
                ?? CoreLibrary.GetFunctions(key1)
                ?? _functions.TryGetValue(key2)
                ?? CoreLibrary.GetFunctions(key2)
                ?? throw new FunctionNotFoundException(FormatForSchema(FUNC05,
                    $"Not found function {caller.GetOutline()}", caller));
    }

    internal void AddFunction(ScriptFunction function) => _functions.Add(function);
    private bool Fail(Exception exception) => _runtime.Exceptions.Fail(exception);

    internal static bool IsMatch(Type type, object value)
        => type.IsInstanceOfType(GetDerived(value));

    internal static object GetDerived(object value)
        => value is IDerived derived ? derived.Derived ?? value : value;
}