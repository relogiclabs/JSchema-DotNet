using System.Text;
using RelogicLabs.JSchema.Engine;
using RelogicLabs.JSchema.Exceptions;
using RelogicLabs.JSchema.Functions;
using RelogicLabs.JSchema.Nodes;
using RelogicLabs.JSchema.Script;
using RelogicLabs.JSchema.Types;
using RelogicLabs.JSchema.Utilities;
using static RelogicLabs.JSchema.Message.ErrorCode;
using static RelogicLabs.JSchema.Types.IEValue;

namespace RelogicLabs.JSchema.Tree;

internal sealed class ScriptFunction : IEFunction
{
    public const string CALLER_HVAR = "+caller";
    public const string TARGET_HVAR = "+target";

    public string Name { get; }
    public GFunction Function { get; }
    public int Arity { get; }
    public Type TargetType { get; }

    public ScriptFunction(string name, GFunction function)
    {
        Name = name;
        Function = function;
        Arity = Function.Variadic ? -1 : Function.Parameters.Length + 1;
        TargetType = typeof(JNode);
    }

    public IList<object>? Prebind<T>(IList<T> arguments) where T : IEValue
    {
        var parameters = Function.Parameters;
        var count = parameters.Length;
        if(Arity == -1 && arguments.Count < count - 1) return null;
        var result = new List<object>(count + 1);
        for(var i = 0; i < count; i++)
        {
            if(parameters[i].Variadic) result.Add(new GArray(
                (IEnumerable<IEValue>) arguments.GetRange(i)));
            else result.Add(arguments[i]);
        }
        return result;
    }

    public object Invoke(JFunction caller, object[] arguments)
    {
        var parameters = Function.Parameters;
        var scope = new ScopeContext(caller.Runtime.ScriptContext);
        scope.Update(CALLER_HVAR, caller);
        scope.Update(TARGET_HVAR, (IEValue) arguments[0]);
        var i = 1;
        foreach(var p in parameters) scope.AddVariable(p.Name, (IEValue) arguments[i++]);
        return Function.IsFuture
            ?  new FutureFunction(() => Invoke(scope))
            : Invoke(scope);
    }

    private bool Invoke(ScopeContext scope)
    {
        try
        {
            var result = Function.Invoke(scope);
            if(ReferenceEquals(result, VOID)) return true;
            if(result is not IEBoolean b)
                throw new InvalidFunctionException(FUNC10,
                    $"Function '{Name}' must return a boolean value");
            return b.Value;
        }
        catch(Exception e) when(e is JsonSchemaException or ScriptInitiatedException)
        {
            throw;
        }
        catch(ScriptCommonException e)
        {
            scope.GetRuntime().Exceptions.Fail(e);
            return false;
        }
    }

    public string GetSignature()
    {
        var builder = new StringBuilder();
        if(Function.IsFuture) builder.Append("future constraint function ");
        else if(Function.IsConstraint) builder.Append("constraint function ");
        else if(Function.IsSubroutine) builder.Append("subroutine function ");
        builder.Append(Name).Append(Function.Parameters.JoinWith(", ", "(", ")"));
        return builder.ToString();
    }

    public override string ToString() => GetSignature();
}