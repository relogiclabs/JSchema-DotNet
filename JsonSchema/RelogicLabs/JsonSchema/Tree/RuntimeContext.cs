using RelogicLabs.JsonSchema.Exceptions;
using RelogicLabs.JsonSchema.Functions;
using RelogicLabs.JsonSchema.Message;
using RelogicLabs.JsonSchema.Types;
using static RelogicLabs.JsonSchema.Message.ErrorCode;

namespace RelogicLabs.JsonSchema.Tree;

public sealed class RuntimeContext
{
    public FunctionRegistry Functions { get; }
    public PragmaRegistry Pragmas { get; }
    public Dictionary<JAlias, JValidator> Definitions { get; }
    public ExceptionRegistry Exceptions { get; }
    public Dictionary<string, FutureValidator> Validators { get; }
    public ReceiverRegistry Receivers { get; }
    public Dictionary<string, object> Storage { get; }
    internal MessageFormatter MessageFormatter { get; }

    internal RuntimeContext(MessageFormatter messageFormatter, bool throwException)
    {
        Functions = new FunctionRegistry(this);
        Pragmas = new PragmaRegistry();
        MessageFormatter = messageFormatter;
        Definitions = new Dictionary<JAlias, JValidator>();
        Exceptions = new ExceptionRegistry(throwException);
        Validators = new Dictionary<string, FutureValidator>();
        Receivers = new ReceiverRegistry();
        Storage = new Dictionary<string, object>();
    }

    public JDefinition AddDefinition(JDefinition definition)
    {
        if(Definitions.TryGetValue(definition.Alias, out var previous))
            throw new DuplicateDefinitionException(MessageFormatter.FormatForSchema(
                DEFI01, $"Duplicate definition of {definition.Alias
                } is found and already defined as {previous.GetOutline()}",
                definition));
        Definitions.Add(definition.Alias, definition.Validator);
        return definition;
    }

    internal bool AreEqual(double value1, double value2)
        => Math.Abs(value1 - value2) < Pragmas.FloatingPointTolerance;

    internal T TryExecute<T>(Func<T> function) => Exceptions.TryExecute(function);
    internal bool FailWith(Exception exception)
    {
        Exceptions.TryThrow(exception);
        Exceptions.TryAdd(exception);
        return false;
    }

    public bool AddValidator(FutureValidator validator)
        => Validators.TryAdd(Guid.NewGuid().ToString(), validator);

    internal bool InvokeValidators()
    {
        var result = true;
        foreach(var v in Validators) result &= v.Value();
        return result;
    }

    public void Clear()
    {
        Exceptions.Clear();
        Storage.Clear();
        Receivers.Clear();
    }
}