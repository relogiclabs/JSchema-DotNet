using RelogicLabs.JsonSchema.Exceptions;
using RelogicLabs.JsonSchema.Message;
using RelogicLabs.JsonSchema.Types;
using static RelogicLabs.JsonSchema.Message.ErrorCode;

namespace RelogicLabs.JsonSchema.Tree;

public class RuntimeContext
{
    private readonly FunctionHelper _functionHelper;
    
    internal MessageFormatter MessageFormatter { get; set; }
    public bool ThrowException { get; set; }
    public Dictionary<string, JPragma> Pragmas { get; }
    public Dictionary<JAlias, JValidator> Definitions { get; }
    public Queue<Exception> ErrorQueue { get; }
    
    public bool IgnoreUnknownProperties 
        => GetPragmaValue<bool>(nameof(IgnoreUnknownProperties));
    public double FloatingPointTolerance 
        => GetPragmaValue<double>(nameof(FloatingPointTolerance));
    public bool IgnoreObjectPropertyOrder 
        => GetPragmaValue<bool>(nameof(IgnoreObjectPropertyOrder));

    internal RuntimeContext(MessageFormatter messageFormatter, bool throwException)
    {
        _functionHelper = new FunctionHelper(this);
        MessageFormatter = messageFormatter;
        ThrowException = throwException;
        Pragmas = new Dictionary<string, JPragma>();
        Definitions = new Dictionary<JAlias, JValidator>();
        ErrorQueue = new Queue<Exception>();
    }
    
    private T GetPragmaValue<T>(string name)
    {
        var pragmaItem = PragmaPreset.From(name);
        Pragmas.TryGetValue(pragmaItem!.Name, out var pragma);
        if(pragma == null) return ((PragmaDescriptor<T>) pragmaItem).DefaultValue;
        return ((IPragmaValue<T>) pragma.Value).Value;
    }
    
    public JPragma AddPragma(JPragma pragma)
    {
        if(Pragmas.ContainsKey(pragma.Name)) 
            throw new DuplicatePragmaException(MessageFormatter.FormatForSchema(
                PRAG03, $"Duplication found for {pragma.ToOutline()}", pragma.Context));
        Pragmas.Add(pragma.Name, pragma);
        return pragma;
    }

    public JInclude AddInclude(JInclude include)
    {
        AddFunctions(include.ClassName, include.Context);
        return include;
    }

    public void AddFunctions(string className, Context? context = null) 
        => _functionHelper.AddFunctions(className, context);

    public bool InvokeFunction(JFunction function, JNode node)
        => _functionHelper.InvokeFunction(function, node);

    public JDefinition AddDefinition(JDefinition definition)
    {
        if(Definitions.TryGetValue(definition.Alias, out var previous))
            throw new DuplicateDefinitionException(MessageFormatter.FormatForSchema(
                DEFI01, $"Duplicate definition of {definition.Alias
                } is found that already defined as {previous.ToOutline()}", 
                definition.Context));
        Definitions.Add(definition.Alias, definition.Validator);
        return definition;
    }
}