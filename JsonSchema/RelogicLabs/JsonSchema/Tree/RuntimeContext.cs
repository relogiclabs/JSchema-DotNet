using RelogicLabs.JsonSchema.Exceptions;
using RelogicLabs.JsonSchema.Message;
using RelogicLabs.JsonSchema.Types;
using static RelogicLabs.JsonSchema.Message.ErrorCode;

namespace RelogicLabs.JsonSchema.Tree;

public sealed class RuntimeContext
{
    private readonly FunctionManager _functionManager;
    private readonly PragmaManager _pragmaManager;
    private int _disableCount;

    internal MessageFormatter MessageFormatter { get; set; }
    public bool ThrowException { get; set; }
    public Dictionary<JAlias, JValidator> Definitions { get; }
    public Queue<Exception> Exceptions { get; }

    public bool IgnoreUndefinedProperties => _pragmaManager.IgnoreUndefinedProperties;
    public double FloatingPointTolerance => _pragmaManager.FloatingPointTolerance;
    public bool IgnoreObjectPropertyOrder => _pragmaManager.IgnoreObjectPropertyOrder;


    internal RuntimeContext(MessageFormatter messageFormatter, bool throwException)
    {
        _functionManager = new FunctionManager(this);
        _pragmaManager = new PragmaManager();
        MessageFormatter = messageFormatter;
        ThrowException = throwException;
        Definitions = new Dictionary<JAlias, JValidator>();
        Exceptions = new Queue<Exception>();
    }

    public JPragma AddPragma(JPragma pragma) => _pragmaManager.AddPragma(pragma);
    public T GetPragmaValue<T>(string name) => _pragmaManager.GetPragmaValue<T>(name);

    public JInclude AddClass(JInclude include)
    {
        AddClass(include.ClassName, include.Context);
        return include;
    }

    public void AddClass(string className, Context? context = null)
        => _functionManager.AddClass(className, context);

    public bool InvokeFunction(JFunction function, JNode target)
        => _functionManager.InvokeFunction(function, target);

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
        => Math.Abs(value1 - value2) < FloatingPointTolerance;

    internal T TryMatch<T>(Func<T> function)
    {
        try
        {
            _disableCount += 1;
            return function();
        }
        finally
        {
            _disableCount -= 1;
        }
    }

    internal bool FailWith(Exception exception)
    {
        if(ThrowException && _disableCount == 0) throw exception;
        if(_disableCount == 0) Exceptions.Enqueue(exception);
        return false;
    }
}