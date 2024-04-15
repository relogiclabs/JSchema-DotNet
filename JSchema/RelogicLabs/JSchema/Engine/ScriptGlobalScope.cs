using RelogicLabs.JSchema.Tree;

namespace RelogicLabs.JSchema.Engine;

internal sealed class ScriptGlobalScope : ScriptScope
{
    private readonly RuntimeContext _runtime;

    public ScriptGlobalScope(RuntimeContext runtime) : base(null)
        => _runtime = runtime;

    public override RuntimeContext GetRuntime() => _runtime;
}