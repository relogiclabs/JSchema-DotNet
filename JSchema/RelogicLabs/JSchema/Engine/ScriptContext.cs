using RelogicLabs.JSchema.Tree;

namespace RelogicLabs.JSchema.Engine;

internal sealed class ScriptContext : ScopeContext
{
    private readonly RuntimeContext _runtime;

    public ScriptContext(RuntimeContext runtime) : base(null)
        => _runtime = runtime;

    public override RuntimeContext GetRuntime() => _runtime;
}