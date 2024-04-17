using RelogicLabs.JSchema.Engine;
using RelogicLabs.JSchema.Script;
using RelogicLabs.JSchema.Types;
using static RelogicLabs.JSchema.Engine.ScriptTreeHelper;
using static RelogicLabs.JSchema.Script.IRFunction;
using static RelogicLabs.JSchema.Message.ErrorCode;

namespace RelogicLabs.JSchema.Library;

internal sealed class LibraryFunction : IRFunction
{
    public GParameter[] Parameters { get; }
    public bool Variadic { get; }
    public FunctionEvaluator Body { get; }

    public LibraryFunction(FunctionEvaluator body, params string[] parameters)
    {
        Parameters = ToParameters(parameters);
        Variadic = HasVariadic(Parameters);
        Body = body;
    }

    private static GParameter[] ToParameters(params string[] names)
        => names.Select(static n => new GParameter(n)).ToArray();

    public ScriptScope Bind(ScriptScope parentScope, List<IEValue> arguments)
    {
        AreCompatible(this, arguments, FNVK03);
        return parentScope;
    }

    public IEValue Invoke(ScriptScope functionScope, List<IEValue> arguments)
        => Body(functionScope, arguments);
}