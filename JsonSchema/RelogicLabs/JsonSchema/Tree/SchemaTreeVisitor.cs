using Antlr4.Runtime.Tree;
using RelogicLabs.JsonSchema.Antlr;
using RelogicLabs.JsonSchema.Collections;
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
    private readonly RuntimeContext _runtime;

    public SchemaTreeVisitor(RuntimeContext runtime)
        => _runtime = runtime;

    public override JNode VisitAggregateSchema(SchemaParser.AggregateSchemaContext context)
        => new JRoot(_relations)
        {
            Context = new Context(context, _runtime),
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
            Context = new Context(context, _runtime),
            Includes = ProcessIncludes(Array.Empty<SchemaParser.IncludeContext>()),
            Value = Visit(context.validator()),
        }.Initialize();

    private IList<JInclude> ProcessIncludes(SchemaParser.IncludeContext[] contexts)
    {
        _runtime.AddClass(typeof(CoreFunctions).FullName!);
        return contexts.Select(i => (JInclude) Visit(i)).ToList();
    }

    public override JNode VisitSchemaBase(SchemaParser.SchemaBaseContext context)
        => Visit(context.validator());

    public override JNode VisitTitle(SchemaParser.TitleContext context)
        => new JTitle(_relations)
        {
            Context = new Context(context, _runtime),
            Title = context.STRING().GetText()
        }.Initialize();

    public override JNode VisitVersion(SchemaParser.VersionContext context)
        => new JVersion(_relations)
        {
            Context = new Context(context, _runtime),
            Version = context.VERSION_NUMBER1().GetText()
        }.Initialize();

    public override JNode VisitInclude(SchemaParser.IncludeContext context)
    {
        var include = new JInclude(_relations)
        {
            Context = new Context(context, _runtime),
            ClassName = context.IDENTIFIER().Select(n => n.GetText()).Join(",")
        }.Initialize();
        return _runtime.AddClass(include);
    }

    public override JNode VisitPragma(SchemaParser.PragmaContext context)
    {
        var nodeContext = new Context(context, _runtime);
        var pragmaName = context.IDENTIFIER().GetText();
        var pragmaValue = (JPrimitive) Visit(context.primitive());
        PragmaDescriptor? descriptor = PragmaDescriptor.From(pragmaName);
        if(descriptor == null) throw new PragmaNotFoundException(MessageFormatter
            .FormatForSchema(PRAG01, $"Invalid pragma {pragmaName.Quote()} with value {
                pragmaValue.GetOutline()} found", nodeContext));
        if(!descriptor.MatchType(pragmaValue.GetType()))
            throw new InvalidPragmaValueException(MessageFormatter.FormatForSchema(
            PRAG02, $"Invalid value {pragmaValue.GetOutline()} for pragma {
                pragmaName.Quote()} found",
            pragmaValue.Context));

        var pragma = new JPragma(_relations)
        {
            Context = nodeContext,
            Name = pragmaName,
            Value = pragmaValue
        }.Initialize();
        return _runtime.AddPragma(pragma);
    }

    public override JNode VisitValidator(SchemaParser.ValidatorContext context)
    {
        if(context.aliasName() != null) return Visit(context.aliasName());
        if(context.validatorMain() != null) return Visit(context.validatorMain());
        throw new InvalidOperationException("Invalid parser state");
    }

    public override JNode VisitDefine(SchemaParser.DefineContext context)
    {
        var definition = new JDefinition(_relations)
        {
            Context = new Context(context, _runtime),
            Alias = (JAlias) Visit(context.aliasName()),
            Validator = (JValidator) Visit(context.validatorMain())
        }.Initialize();
        return _runtime.AddDefinition(definition);
    }

    public override JNode VisitAliasName(SchemaParser.AliasNameContext context)
        => new JAlias(_relations)
        {
            Context = new Context(context, _runtime),
            Name = context.ALIAS().GetText()
        };

    public override JNode VisitValue(SchemaParser.ValueContext context)
    {
        if(context.primitive() != null) return Visit(context.primitive());
        if(context.array() != null) return Visit(context.array());
        if(context.@object() != null) return Visit(context.@object());
        throw new InvalidOperationException("Invalid parser state");
    }

    public override JNode VisitValidatorMain(SchemaParser.ValidatorMainContext context)
        => new JValidator(_relations)
        {
            Context = new Context(context, _runtime),
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
            Context = new Context(context, _runtime),
            Properties = ProcessProperties(context.property())
        }.Initialize();

    private  IIndexMap<string,JProperty> ProcessProperties(SchemaParser.PropertyContext[] contexts)
    {
        List<JProperty> properties = contexts.Select(p => (JProperty) Visit(p)).ToList();
        return JProperty.CheckForDuplicate(properties, PROP04).ToIndexMap();
    }

    public override JNode VisitProperty(SchemaParser.PropertyContext context)
        => new JProperty(_relations)
        {
            Context = new Context(context, _runtime),
            Key = context.STRING().GetText()[1..^1],
            Value = Visit(context.validator())
        }.Initialize();

    public override JNode VisitArray(SchemaParser.ArrayContext context)
        => new JArray(_relations)
        {
            Context = new Context(context, _runtime),
            Elements = context.validator().Select(Visit).ToList().AsReadOnly()
        }.Initialize();

    public override JNode VisitDatatype(SchemaParser.DatatypeContext context)
        => new JDataType(_relations)
        {
            Context = new Context(context, _runtime),
            JsonType = JsonType.From(context.DATATYPE()),
            Alias = (JAlias) Visit(context.aliasName()),
            Nested = context.STAR() != null
        }.Initialize();

    public override JNode VisitFunction(SchemaParser.FunctionContext context)
        => new JFunction(_relations)
        {
            Context = new Context(context, _runtime),
            Name = context.FUNCTION().GetText(),
            Arguments = context.value().Select(Visit).ToList().AsReadOnly(),
            Nested = context.STAR() != null
        }.Initialize();

    public override JNode VisitPrimitiveTrue(SchemaParser.PrimitiveTrueContext context)
        => new JBoolean(_relations)
        {
            Context = new Context(context, _runtime),
            Value = true
        }.Initialize();

    public override JNode VisitPrimitiveFalse(SchemaParser.PrimitiveFalseContext context)
        => new JBoolean(_relations)
        {
            Context = new Context(context, _runtime),
            Value = false
        }.Initialize();

    public override JNode VisitPrimitiveString(SchemaParser.PrimitiveStringContext context)
        => new JString(_relations)
        {
            Context = new Context(context, _runtime),
            Value = context.STRING().GetText().ToEncoded()
        }.Initialize();

    public override JNode VisitPrimitiveInteger(SchemaParser.PrimitiveIntegerContext context)
        => new JInteger(_relations)
        {
            Context = new Context(context, _runtime),
            Value = Convert.ToInt64(context.INTEGER().GetText())
        }.Initialize();

    public override JNode VisitPrimitiveFloat(SchemaParser.PrimitiveFloatContext context)
        => new JFloat(_relations)
        {
            Context = new Context(context, _runtime),
            Value = Convert.ToDouble(context.FLOAT().GetText())
        }.Initialize();

    public override JNode VisitPrimitiveDouble(SchemaParser.PrimitiveDoubleContext context)
        => new JDouble(_relations)
        {
            Context = new Context(context, _runtime),
            Value = Convert.ToDouble(context.DOUBLE().GetText())
        }.Initialize();

    public override JNode VisitPrimitiveNull(SchemaParser.PrimitiveNullContext context)
        => new JNull(_relations) { Context = new Context(context, _runtime) }
            .Initialize();

    public override JNode VisitPrimitiveUndefined(SchemaParser.PrimitiveUndefinedContext context)
        => new JUndefined(_relations) { Context = new Context(context, _runtime) }
            .Initialize();

    public override JNode Visit(IParseTree? tree)
        => tree == null ? null! : base.Visit(tree);
}