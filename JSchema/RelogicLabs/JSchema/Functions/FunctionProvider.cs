using RelogicLabs.JSchema.Tree;
using RelogicLabs.JSchema.Nodes;

namespace RelogicLabs.JSchema.Functions;

public abstract class FunctionProvider
{
    public RuntimeContext Runtime { get; }
    public JFunction Caller { get; set; } = null!;

    protected FunctionProvider(RuntimeContext runtime) => Runtime = runtime;

    protected bool Fail(Exception exception)
        => Runtime.Exceptions.Fail(exception);
}