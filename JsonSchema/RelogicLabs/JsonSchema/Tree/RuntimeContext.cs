using RelogicLabs.JsonSchema.Exceptions;
using RelogicLabs.JsonSchema.Message;
using RelogicLabs.JsonSchema.Types;
using RelogicLabs.JsonSchema.Utilities;
using static RelogicLabs.JsonSchema.Message.ErrorCode;

namespace RelogicLabs.JsonSchema.Tree;

public sealed class RuntimeContext
{
    private int _disableException;
    private readonly Dictionary<string, object> _memoryMap;

    internal MessageFormatter MessageFormatter { get; set; }
    public bool ThrowException { get; set; }
    public Dictionary<JAlias, JValidator> Definitions { get; }
    public PragmaRegistry Pragmas { get; }
    public FunctionRegistry Functions { get; }
    public Queue<Exception> Exceptions { get; }

    internal RuntimeContext(MessageFormatter messageFormatter, bool throwException)
    {
        _memoryMap = new Dictionary<string, object>();
        Functions = new FunctionRegistry(this);
        Pragmas = new PragmaRegistry();
        MessageFormatter = messageFormatter;
        ThrowException = throwException;
        Definitions = new Dictionary<JAlias, JValidator>();
        Exceptions = new Queue<Exception>();
    }

    public JDefinition AddDefinition(JDefinition definition)
    {
        if(Definitions.TryGetValue(definition.Alias, out var previous))
            throw new DuplicateDefinitionException(MessageFormatter.FormatForSchema(
                DEFI01, $"Duplicate definition of {definition.Alias
                } is found and already defined as {previous.GetOutline()}",
                definition.Context));
        Definitions.Add(definition.Alias, definition.Validator);
        return definition;
    }

    internal bool AreEqual(double value1, double value2)
        => Math.Abs(value1 - value2) < Pragmas.FloatingPointTolerance;

    internal T TryMatch<T>(Func<T> function)
    {
        try
        {
            _disableException += 1;
            return function();
        }
        finally
        {
            _disableException -= 1;
        }
    }

    internal bool FailWith(Exception exception)
    {
        if(ThrowException && _disableException == 0) throw exception;
        if(_disableException == 0) Exceptions.Enqueue(exception);
        return false;
    }

    public void Store(string name, object value) => _memoryMap[name] = value;
    public object? Retrieve(string name) => _memoryMap.GetValue(name);
}