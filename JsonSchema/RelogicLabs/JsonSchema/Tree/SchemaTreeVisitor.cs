using Antlr4.Runtime.Tree;
using RelogicLabs.JsonSchema.Antlr;
using RelogicLabs.JsonSchema.Exceptions;
using RelogicLabs.JsonSchema.Functions;
using RelogicLabs.JsonSchema.Message;
using RelogicLabs.JsonSchema.Types;
using RelogicLabs.JsonSchema.Utilities;
using static RelogicLabs.JsonSchema.Message.ErrorCode;

namespace RelogicLabs.JsonSchema.Tree;

internal class SchemaTreeVisitor : SchemaParserBaseVisitor<JNode>
{
    private readonly Dictionary<JNode, JNode> _relations = new();
    private readonly RuntimeContext _context;

    public SchemaTreeVisitor(RuntimeContext context) 
        => _context = context;


    public override JNode VisitAggregateSchema(SchemaParser.AggregateSchemaContext context)
        => new JRoot(_relations)
        {
            Context = new Context(context, _context),
            Title = (JTitle) Visit(context.title()),
            Version = (JVersion) Visit(context.version()),
            Includes = ProcessIncludes(context.include()).AsReadOnly(),
            Pragmas = context.pragma().Select(p => (JPragma) Visit(p)).ToList().AsReadOnly(),
            Definitions = context.define().Select(d => (JDefinition) Visit(d)).ToList().AsReadOnly(),
            Value = Visit(context.schemaBase())
        }.Initialize();

    public override JNode VisitCoreSchema(SchemaParser.CoreSchemaContext context)
        => new JRoot(_relations)
        {
            Context = new Context(context, _context),
            Includes = ProcessIncludes(Array.Empty<SchemaParser.IncludeContext>()),
            Value = Visit(context.validator()),
        }.Initialize();

    private IList<JInclude> ProcessIncludes(SchemaParser.IncludeContext[] contexts)
    {
        _context.AddFunctions(typeof(CoreFunctions).FullName!);
        return contexts.Select(i => (JInclude) Visit(i)).ToList();
    }
    
    public override JNode VisitSchemaBase(SchemaParser.SchemaBaseContext context)
        => Visit(context.validator());

    public override JNode VisitTitle(SchemaParser.TitleContext context)
        => new JTitle(_relations)
        {
            Context = new Context(context, _context),
            Title = context.STRING().GetText()
        }.Initialize();

    public override JNode VisitVersion(SchemaParser.VersionContext context)
        => new JVersion(_relations)
        {
            Context = new Context(context, _context),
            Version = context.VERSION_NUMBER1().GetText()
        }.Initialize();

    public override JNode VisitInclude(SchemaParser.IncludeContext context)
    {
        var include = new JInclude(_relations)
        {
            Context = new Context(context, _context),
            ClassName = context.IDENTIFIER().Select(n => n.GetText()).ToString(",")
        }.Initialize();
        return _context.AddInclude(include);
    }
    
    public override JNode VisitPragma(SchemaParser.PragmaContext context)
    {
        var nodeContext = new Context(context, _context);
        var pragmaName = context.IDENTIFIER().GetText();
        var pragmaValue = (JPrimitive) Visit(context.primitive());
        PragmaPreset? descriptor = PragmaPreset.From(pragmaName);
        if(descriptor == null) throw new PragmaNotFoundException(MessageFormatter
            .FormatForSchema(PRAG01, $"Invalid pragma {pragmaName.DoubleQuote()} with value {
                pragmaValue.ToOutline()} found", nodeContext));
        if(!descriptor.MatchType(pragmaValue.GetType())) 
            throw new InvalidPragmaValueException(MessageFormatter.FormatForSchema(
            PRAG02, $"Invalid value {pragmaValue.ToOutline()} for pragma {
                pragmaName.DoubleQuote()} found", 
            pragmaValue.Context));
        
        var pragma = new JPragma(_relations)
        {
            Context = nodeContext,
            Name = pragmaName,
            Value = pragmaValue
        }.Initialize();
        return _context.AddPragma(pragma);
    }

    public override JNode VisitValidator(SchemaParser.ValidatorContext context)
    {
        if(context.aliasName() != null) return Visit(context.aliasName());
        if(context.validatorMain() != null) return Visit(context.validatorMain());
        throw new InvalidOperationException("Invalid Parser State");
    }

    public override JNode VisitDefine(SchemaParser.DefineContext context)
    {
        var definition = new JDefinition(_relations)
        {
            Context = new Context(context, _context),
            Alias = (JAlias) Visit(context.aliasName()),
            Validator = (JValidator) Visit(context.validatorMain())
        }.Initialize();
        return _context.AddDefinition(definition);
    }

    public override JNode VisitAliasName(SchemaParser.AliasNameContext context) 
        => new JAlias(_relations)
        {
            Context = new Context(context, _context),
            Name = context.ALIAS().GetText()
        };

    public override JNode VisitValue(SchemaParser.ValueContext context)
    {
        if(context.primitive() != null) return Visit(context.primitive());
        if(context.array() != null) return Visit(context.array());
        if(context.@object() != null) return Visit(context.@object());
        throw new InvalidOperationException("Invalid Parser State");
    }

    public override JNode VisitValidatorMain(SchemaParser.ValidatorMainContext context)
        => new JValidator(_relations)
        {
            Context = new Context(context, _context),
            Value = Visit(context.value()),
            Functions = context.function().Select(ctx => (JFunction) Visit(ctx))
                .ToList().AsReadOnly(),
            DataTypes = context.datatype().Select(ctx => (JDataType) Visit(ctx))
                .ToList().AsReadOnly(),
            Optional = context.OPTIONAL() != null
        }.Initialize();

    public override JNode VisitObject(SchemaParser.ObjectContext context)
        => new JObject(_relations)
        {
            Context = new Context(context, _context),
            Properties = CheckProperties(context.property()).ToProperties()
        }.Initialize();

    private IEnumerable<JProperty> CheckProperties(SchemaParser.PropertyContext[] contexts)
    {
        List<JProperty> properties = contexts.Select(p => (JProperty) Visit(p)).ToList();
        var groups = properties.GroupBy(p => p.Key).Where(g => g.Count() > 1).ToList();
        if(groups.Count == 0) return properties;
        JProperty property = groups.First().First();
        throw new DuplicatePropertyKeyException(MessageFormatter.FormatForSchema(
            PROP04, $"Multiple key with name {groups.First().Key.DoubleQuote()} found", 
            property.Context));
    }
    
    public override JNode VisitProperty(SchemaParser.PropertyContext context)
        => new JProperty(_relations)
        {
            Context = new Context(context, _context),
            Key = context.STRING().GetText()[1..^1],
            Value = Visit(context.validator())
        }.Initialize();

    public override JNode VisitArray(SchemaParser.ArrayContext context)
        => new JArray(_relations)
        {
            Context = new Context(context, _context),
            Elements = context.validator().Select(Visit).ToList().AsReadOnly()
        }.Initialize();

    public override JNode VisitDatatype(SchemaParser.DatatypeContext context)
        => new JDataType(_relations)
        {
            Context = new Context(context, _context),
            JsonType = JsonType.From(context.DATATYPE().GetText()),
            Alias = (JAlias) Visit(context.aliasName()),
            Nested = context.STAR() != null
        }.Initialize();

    public override JNode VisitFunction(SchemaParser.FunctionContext context)
        => new JFunction(_relations)
        {
            Context = new Context(context, _context),
            Name = context.FUNCTION().GetText(),
            Arguments = context.value().Select(Visit).ToList().AsReadOnly(),
            Nested = context.STAR() != null
        }.Initialize();

    public override JNode VisitPrimitiveTrue(SchemaParser.PrimitiveTrueContext context)
        => new JBoolean(_relations)
        {
            Context = new Context(context, _context),
            Value = true
        }.Initialize();

    public override JNode VisitPrimitiveFalse(SchemaParser.PrimitiveFalseContext context)
        => new JBoolean(_relations)
        {
            Context = new Context(context, _context),
            Value = false
        }.Initialize();

    public override JNode VisitPrimitiveString(SchemaParser.PrimitiveStringContext context)
        => new JString(_relations)
        {
            Context = new Context(context, _context),
            Value = context.STRING().GetText().ToEncoded()
        }.Initialize();

    public override JNode VisitPrimitiveInteger(SchemaParser.PrimitiveIntegerContext context)
        => new JInteger(_relations)
        {
            Context = new Context(context, _context),
            Value = Convert.ToInt64(context.INTEGER().GetText())
        }.Initialize();

    public override JNode VisitPrimitiveFloat(SchemaParser.PrimitiveFloatContext context)
        => new JFloat(_relations)
        {
            Context = new Context(context, _context),
            Value = Convert.ToDouble(context.FLOAT().GetText())
        }.Initialize();

    public override JNode VisitPrimitiveDouble(SchemaParser.PrimitiveDoubleContext context)
        => new JDouble(_relations)
        {
            Context = new Context(context, _context),
            Value = Convert.ToDouble(context.DOUBLE().GetText())
        }.Initialize();

    public override JNode VisitPrimitiveNull(SchemaParser.PrimitiveNullContext context)
        => new JNull(_relations) { Context = new Context(context, _context) }
            .Initialize();

    public override JNode VisitPrimitiveUnknown(SchemaParser.PrimitiveUnknownContext context)
        => new JUnknown(_relations) { Context = new Context(context, _context) }
            .Initialize();

    public override JNode Visit(IParseTree? tree) 
        => tree == null ? null! : base.Visit(tree);
}