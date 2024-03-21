using RelogicLabs.JSchema.Engine;
using RelogicLabs.JSchema.Script;
using RelogicLabs.JSchema.Types;
using static RelogicLabs.JSchema.Engine.ScriptTreeHelper;
using static RelogicLabs.JSchema.Script.IRFunction;
using static RelogicLabs.JSchema.Message.ErrorCode;

namespace RelogicLabs.JSchema.Library;

internal sealed class NFunction : IRFunction
{
    public GParameter[] Parameters { get; }
    public bool Variadic { get; }
    public NEvaluator Body { get; }

    public NFunction(NEvaluator body, params string[] parameters)
    {
        Parameters = ToParameters(parameters);
        Variadic = HasVariadic(Parameters);
        Body = body;
    }

    private static GParameter[] ToParameters(params string[] names)
        => names.Select(static n => new GParameter(n)).ToArray();

    public ScopeContext Bind(ScopeContext parentScope, List<IEValue> arguments)
    {
        AreCompatible(this, arguments, FUNS06);
        return parentScope;
    }

    public IEValue Invoke(ScopeContext functionScope, List<IEValue> arguments)
        => Body(functionScope, arguments);
}