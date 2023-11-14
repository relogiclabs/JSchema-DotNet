using RelogicLabs.JsonSchema.Exceptions;
using RelogicLabs.JsonSchema.Tree;
using RelogicLabs.JsonSchema.Types;

namespace RelogicLabs.JsonSchema.Functions;

public abstract class FunctionBase
{
    public RuntimeContext Runtime { get; }
    public JFunction Function { get; set; } = null!;

    protected FunctionBase(RuntimeContext runtime) => Runtime = runtime;

    public bool FailWith(JsonSchemaException exception)
        => Runtime.FailWith(exception);
}