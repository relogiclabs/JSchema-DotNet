using RelogicLabs.JSchema.Exceptions;
using RelogicLabs.JSchema.Script;
using RelogicLabs.JSchema.Tree;
using RelogicLabs.JSchema.Types;
using RelogicLabs.JSchema.Utilities;
using static RelogicLabs.JSchema.Library.ScriptLibrary;
using static RelogicLabs.JSchema.Message.ErrorCode;
using static RelogicLabs.JSchema.Script.GFunction;

namespace RelogicLabs.JSchema.Engine;

internal class ScopeContext
{
    public ScopeContext? Parent { get; }
    private readonly Dictionary<string, IEValue> _symbols;

    public ScopeContext(ScopeContext? parent)
    {
        Parent = parent;
        _symbols = new Dictionary<string, IEValue>();
    }

    public GReference AddVariable(string name, IEValue value)
    {
        var reference = new GReference(value);
        var result = _symbols.TryAdd(name, reference);
        if(result) return reference;
        throw new ScriptVariableException(VARD01,
            $"Variable '{name}' already defined in the scope");
    }

    public void AddFunction(string name, GFunction function)
    {
        var result = _symbols.TryAdd(name, function);
        if(result) return;
        if(name.StartsWith(ConstraintPrefix))
            throw FailOnDuplicateDefinition(FUNS02, "Constraint", name);
        throw FailOnDuplicateDefinition(FUNS03, "Subroutine", name);
    }

    private static ScriptFunctionException FailOnDuplicateDefinition(string code,
        string functionType, string name) => new(code, $"{functionType} function '{
            name.SubstringBefore('#')}' with matching parameter(s) already defined");

    public IEValue Resolve(string name)
    {
        var current = this;
        do
        {
            current._symbols.TryGetValue(name, out var value);
            if(value != null) return value;
            current = current.Parent;
        } while(current != null);
        return ResolveStatic(name);
    }

    public virtual RuntimeContext GetRuntime()
    {
        var current = this;
        while(current.Parent != null) current = current.Parent;
        return current.GetRuntime();
    }

    public void Update(string name, IEValue value)
        => _symbols[name] = value;
}