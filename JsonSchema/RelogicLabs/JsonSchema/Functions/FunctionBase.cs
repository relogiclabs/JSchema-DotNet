using RelogicLabs.JsonSchema.Exceptions;
using RelogicLabs.JsonSchema.Tree;
using RelogicLabs.JsonSchema.Types;

namespace RelogicLabs.JsonSchema.Functions;

public abstract class FunctionBase
{
    public required RuntimeContext Runtime { get; init; }
    public JFunction Function { get; set; } = null!;
    
    protected FunctionBase(RuntimeContext runtime) => Runtime = runtime;
    
    protected bool FailWith(JsonSchemaException exception)
        => Runtime.FailWith(exception);
}