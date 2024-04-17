using RelogicLabs.JSchema.Engine;
using RelogicLabs.JSchema.Types;

namespace RelogicLabs.JSchema.Script;

internal interface IRFunction : IEValue
{
    GParameter[] Parameters { get; }
    bool Variadic { get; }
    ScriptScope Bind(ScriptScope parentScope, List<IEValue> arguments);
    IEValue Invoke(ScriptScope functionScope, List<IEValue> arguments);

    static bool HasVariadic(GParameter[] parameters)
        => parameters.Length != 0 && parameters[^1].Variadic;
}