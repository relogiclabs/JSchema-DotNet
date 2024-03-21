using RelogicLabs.JSchema.Engine;
using RelogicLabs.JSchema.Types;

namespace RelogicLabs.JSchema.Script;

internal interface IRFunction : IEValue
{
    GParameter[] Parameters { get; }
    bool Variadic { get; }
    ScopeContext Bind(ScopeContext parentScope, List<IEValue> arguments);
    IEValue Invoke(ScopeContext functionScope, List<IEValue> arguments);

    static bool HasVariadic(GParameter[] parameters)
        => parameters.Length != 0 && parameters[^1].Variadic;
}