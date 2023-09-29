using RelogicLabs.JsonSchema.Exceptions;
using RelogicLabs.JsonSchema.Message;
using RelogicLabs.JsonSchema.Types;
using static RelogicLabs.JsonSchema.Message.ErrorCode;

namespace RelogicLabs.JsonSchema.Tree;

public class RuntimeContext
{
    private readonly FunctionManager _functionManager;
    
    internal MessageFormatter MessageFormatter { get; set; }
    public bool ThrowException { get; set; }
    public Dictionary<string, JPragma> Pragmas { get; }
    public Dictionary<JAlias, JValidator> Definitions { get; }
    public Queue<Exception> Exceptions { get; }
    
    public bool IgnoreUndefinedProperties 
        => GetPragmaValue<bool>(nameof(IgnoreUndefinedProperties));
    public double FloatingPointTolerance 
        => GetPragmaValue<double>(nameof(FloatingPointTolerance));
    public bool IgnoreObjectPropertyOrder 
        => GetPragmaValue<bool>(nameof(IgnoreObjectPropertyOrder));

    internal RuntimeContext(MessageFormatter messageFormatter, bool throwException)
    {
        _functionManager = new FunctionManager(this);
        MessageFormatter = messageFormatter;
        ThrowException = throwException;
        Pragmas = new Dictionary<string, JPragma>();
        Definitions = new Dictionary<JAlias, JValidator>();
        Exceptions = new Queue<Exception>();
    }
    
    private T GetPragmaValue<T>(string name)
    {
        var entry = PragmaDescriptor.From(name);
        Pragmas.TryGetValue(entry!.Name, out var pragma);
        return pragma == null ? ((PragmaProfile<T>) entry).DefaultValue 
            : ((IPragmaValue<T>) pragma.Value).Value;
    }
    
    public JPragma AddPragma(JPragma pragma)
    {
        if(Pragmas.ContainsKey(pragma.Name)) 
            throw new DuplicatePragmaException(MessageFormatter.FormatForSchema(
                PRAG03, $"Duplication found for {pragma.GetOutline()}", pragma.Context));
        Pragmas.Add(pragma.Name, pragma);
        return pragma;
    }

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
    
    internal bool FailWith(Exception exception)
    {
        if(ThrowException) throw exception;
        Exceptions.Enqueue(exception);
        return false;
    }
}