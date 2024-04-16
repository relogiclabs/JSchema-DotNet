using RelogicLabs.JSchema.Nodes;
using RelogicLabs.JSchema.Tree;

namespace RelogicLabs.JSchema.Functions;

public abstract class FunctionProvider
{
    public RuntimeContext Runtime { get; set; } = null!;
    public JFunction Caller { get; set; } = null!;

    protected bool Fail(Exception exception)
        => Runtime.Exceptions.Fail(exception);
}