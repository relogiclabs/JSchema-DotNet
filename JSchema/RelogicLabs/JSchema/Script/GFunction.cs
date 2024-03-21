using RelogicLabs.JSchema.Engine;
using RelogicLabs.JSchema.Types;
using RelogicLabs.JSchema.Utilities;
using static RelogicLabs.JSchema.Engine.ScriptTreeHelper;
using static RelogicLabs.JSchema.Script.IRFunction;
using static RelogicLabs.JSchema.Message.ErrorCode;
using static RelogicLabs.JSchema.Types.IEValue;
using static RelogicLabs.JSchema.Utilities.CommonUtilities;

namespace RelogicLabs.JSchema.Script;

internal sealed class GFunction : IRFunction
{
    public const string ConstraintPrefix = "@";
    public const int ConstraintMode = 1;
    public const int FutureMode = 3;
    public const int SubroutineMode = 4;

    public GParameter[] Parameters { get; }
    public bool Variadic { get; }
    public Evaluator Body { get; }
    public int Mode { get; }
    public bool IsConstraint => HasFlag(Mode, ConstraintMode);
    public bool IsFuture => HasFlag(Mode, FutureMode);
    public bool IsSubroutine => HasFlag(Mode, SubroutineMode);

    public GFunction(GParameter[] parameters, Evaluator body, int mode)
    {
        Parameters = parameters;
        Variadic = HasVariadic(parameters);
        Body = body;
        Mode = mode;
    }

    public ScopeContext Bind(ScopeContext parentScope, List<IEValue> arguments)
    {
        AreCompatible(this, arguments, FUNS05);
        var scope = new ScopeContext(parentScope);
        var i = 0;
        foreach(var p in Parameters) scope.AddVariable(p.Name, p.Variadic
                ? new GArray(arguments.GetRange(i)) : arguments[i++]);
        return scope;
    }

    public IEValue Invoke(ScopeContext functionScope, List<IEValue> arguments)
        => Invoke(functionScope);

    public IEValue Invoke(ScopeContext functionScope)
    {
        var result = Body(functionScope);
        if(result is GControl ctrl) return ctrl.Value;
        return VOID;
    }
}