using RelogicLabs.JSchema.Engine;
using RelogicLabs.JSchema.Exceptions;
using RelogicLabs.JSchema.Functions;
using RelogicLabs.JSchema.Message;
using RelogicLabs.JSchema.Nodes;
using static RelogicLabs.JSchema.Message.ErrorCode;
using static RelogicLabs.JSchema.Message.MessageFormatter;

namespace RelogicLabs.JSchema.Tree;

public sealed class RuntimeContext
{
    public FunctionRegistry Functions { get; }
    public PragmaRegistry Pragmas { get; }
    public Dictionary<JAlias, JValidator> Definitions { get; }
    public ExceptionRegistry Exceptions { get; }
    public Dictionary<string, FutureFunction> Futures { get; }
    public ReceiverRegistry Receivers { get; }
    public Dictionary<string, object> Storage { get; }
    public MessageFormatter MessageFormatter { get; }
    internal ScriptContext ScriptContext { get; }

    internal RuntimeContext(MessageFormatter messageFormatter, bool throwException)
    {
        Functions = new FunctionRegistry(this);
        Pragmas = new PragmaRegistry();
        MessageFormatter = messageFormatter;
        Definitions = new Dictionary<JAlias, JValidator>();
        Exceptions = new ExceptionRegistry(throwException);
        Futures = new Dictionary<string, FutureFunction>();
        Receivers = new ReceiverRegistry();
        Storage = new Dictionary<string, object>();
        ScriptContext = new ScriptContext(this);
    }

    public JDefinition AddDefinition(JDefinition definition)
    {
        if(Definitions.TryGetValue(definition.Alias, out var previous))
            throw new DuplicateDefinitionException(FormatForSchema(DEFI01,
                $"Duplicate definition of {definition.Alias
                } is found and already defined as {previous.GetOutline()}",
                definition));
        Definitions.Add(definition.Alias, definition.Validator);
        return definition;
    }

    internal bool AreEqual(double value1, double value2)
        => Math.Abs(value1 - value2) < Pragmas.FloatingPointTolerance;

    public bool AddFuture(FutureFunction future)
        => Futures.TryAdd(Guid.NewGuid().ToString(), future);

    internal bool InvokeFutures()
    {
        var result = true;
        foreach(var f in Futures) result &= f.Value();
        return result;
    }

    public void Clear()
    {
        Exceptions.Clear();
        Storage.Clear();
        Receivers.Clear();
    }
}