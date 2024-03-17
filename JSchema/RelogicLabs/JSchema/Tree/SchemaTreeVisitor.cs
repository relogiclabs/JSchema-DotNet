using Antlr4.Runtime.Tree;
using RelogicLabs.JsonSchema.Antlr;
using RelogicLabs.JsonSchema.Collections;
using RelogicLabs.JsonSchema.Functions;
using RelogicLabs.JsonSchema.Types;
using RelogicLabs.JsonSchema.Utilities;
using static RelogicLabs.JsonSchema.Message.ErrorCode;
using static RelogicLabs.JsonSchema.Utilities.CommonUtilities;

namespace RelogicLabs.JsonSchema.Tree;

internal sealed class SchemaTreeVisitor : SchemaParserBaseVisitor<JNode>
{
    private readonly Dictionary<JNode, JNode> _relations = new();
    private readonly RuntimeContext _runtime;

    public SchemaTreeVisitor(RuntimeContext runtime)
        => _runtime = runtime;

    public override JNode VisitAggregateSchema(SchemaParser.AggregateSchemaContext context)
        => new JRoot.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Title = (JTitle) Visit(context.title()),
            Version = (JVersion) Visit(context.version()),
            Includes = ProcessIncludes(context.include()).AsReadOnly(),
            Pragmas = context.pragma().Select(p => (JPragma) Visit(p)).ToList().AsReadOnly(),
            Definitions = context.define().Select(d => (JDefinition) Visit(d)).ToList().AsReadOnly(),
            Value = Visit(context.schemaBase())
        }.Build();

    public override JNode VisitCoreSchema(SchemaParser.CoreSchemaContext context)
        => new JRoot.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Includes = ProcessIncludes(Array.Empty<SchemaParser.IncludeContext>()),
            Value = Visit(context.validator())
        }.Build();

    private List<JInclude> ProcessIncludes(SchemaParser.IncludeContext[] contexts)
    {
        _runtime.Functions.AddClass(typeof(CoreFunctions).FullName!);
        return contexts.Select(i => (JInclude) Visit(i)).ToList();
    }

    public override JNode VisitSchemaBase(SchemaParser.SchemaBaseContext context)
        => Visit(context.validator());

    public override JNode VisitTitle(SchemaParser.TitleContext context)
        => new JTitle.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Title = context.STRING().GetText()
        }.Build();

    public override JNode VisitVersion(SchemaParser.VersionContext context)
        => new JVersion.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Version = context.VERSION_NUMBER1().GetText()
        }.Build();

    public override JNode VisitInclude(SchemaParser.IncludeContext context)
    {
        var include = new JInclude.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            ClassName = context.IDENTIFIER().Select(n => n.GetText()).Join(",")
        }.Build();
        return _runtime.Functions.AddClass(include);
    }

    public override JNode VisitPragma(SchemaParser.PragmaContext context)
    {
        var pragma = new JPragma.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Name = context.IDENTIFIER().GetText(),
            Value = (JPrimitive) Visit(context.primitive())
        }.Build();
        return _runtime.Pragmas.AddPragma(pragma);
    }

    public override JNode VisitValidator(SchemaParser.ValidatorContext context)
    {
        if(context.alias() != null) return Visit(context.alias());
        if(context.validatorMain() != null) return Visit(context.validatorMain());
        throw new InvalidOperationException("Invalid parser state");
    }

    public override JNode VisitDefine(SchemaParser.DefineContext context)
    {
        var definition = new JDefinition.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Alias = (JAlias) Visit(context.alias()),
            Validator = (JValidator) Visit(context.validatorMain())
        }.Build();
        return _runtime.AddDefinition(definition);
    }

    public override JNode VisitAlias(SchemaParser.AliasContext context)
        => new JAlias.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Name = context.ALIAS().GetText()
        }.Build();

    public override JNode VisitValue(SchemaParser.ValueContext context)
    {
        if(context.primitive() != null) return Visit(context.primitive());
        if(context.array() != null) return Visit(context.array());
        if(context.@object() != null) return Visit(context.@object());
        throw new InvalidOperationException("Invalid parser state");
    }

    public override JNode VisitValidatorMain(SchemaParser.ValidatorMainContext context)
        => new JValidator.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Value = Visit(context.value()),
            Functions = context.function().Select(ctx => (JFunction) Visit(ctx))
                .ToList().AsReadOnly(),
            DataTypes = context.datatype().Select(ctx => (JDataType) Visit(ctx))
                .ToList().AsReadOnly(),
            Receivers = context.receiver().Select(ctx => (JReceiver) Visit(ctx))
                .ToList().AsReadOnly(),
            Optional = context.OPTIONAL() != null
        }.Build();

    public override JNode VisitObject(SchemaParser.ObjectContext context)
        => new JObject.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Properties = ProcessProperties(context.property())
        }.Build();

    private  IIndexMap<string,JProperty> ProcessProperties(SchemaParser.PropertyContext[] contexts)
    {
        List<JProperty> properties = contexts.Select(p => (JProperty) Visit(p)).ToList();
        return RequireUniqueness(properties, PROP04).ToIndexMap().AsReadOnly();
    }

    public override JNode VisitProperty(SchemaParser.PropertyContext context)
        => new JProperty.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Key = context.STRING().GetText()[1..^1],
            Value = Visit(context.validator())
        }.Build();

    public override JNode VisitArray(SchemaParser.ArrayContext context)
        => new JArray.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Elements = context.validator().Select(Visit).ToList().AsReadOnly()
        }.Build();

    public override JNode VisitDatatype(SchemaParser.DatatypeContext context)
        => new JDataType.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            JsonType = JsonType.From(context.DATATYPE()),
            Nested = context.STAR() != null,
            Alias = (JAlias) Visit(context.alias())
        }.Build();

    public override JNode VisitFunction(SchemaParser.FunctionContext context)
        => new JFunction.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Name = context.FUNCTION().GetText(),
            Nested = context.STAR() != null,
            Arguments = context.argument().Select(Visit).ToList().AsReadOnly()
        }.Build();

    public override JNode VisitArgument(SchemaParser.ArgumentContext context)
    {
        if(context.value() != null) return Visit(context.value());
        if(context.receiver() != null) return Visit(context.receiver());
        throw new InvalidOperationException("Invalid parser state");
    }

    public override JNode VisitReceiver(SchemaParser.ReceiverContext context)
        => new JReceiver.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Name = context.RECEIVER().GetText()
        }.Build();

    public override JNode VisitPrimitiveTrue(SchemaParser.PrimitiveTrueContext context)
        => new JBoolean.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Value = true
        }.Build();

    public override JNode VisitPrimitiveFalse(SchemaParser.PrimitiveFalseContext context)
        => new JBoolean.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Value = false
        }.Build();

    public override JNode VisitPrimitiveString(SchemaParser.PrimitiveStringContext context)
        => new JString.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Value = context.STRING().GetText().ToEncoded()
        }.Build();

    public override JNode VisitPrimitiveInteger(SchemaParser.PrimitiveIntegerContext context)
        => new JInteger.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Value = Convert.ToInt64(context.INTEGER().GetText())
        }.Build();

    public override JNode VisitPrimitiveFloat(SchemaParser.PrimitiveFloatContext context)
        => new JFloat.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Value = Convert.ToDouble(context.FLOAT().GetText())
        }.Build();

    public override JNode VisitPrimitiveDouble(SchemaParser.PrimitiveDoubleContext context)
        => new JDouble.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Value = Convert.ToDouble(context.DOUBLE().GetText())
        }.Build();

    public override JNode VisitPrimitiveNull(SchemaParser.PrimitiveNullContext context)
        => new JNull.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime)
        }.Build();

    public override JNode VisitPrimitiveUndefined(SchemaParser.PrimitiveUndefinedContext context)
        => new JUndefined.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime)
        }.Build();

    public override JNode Visit(IParseTree? tree)
        => tree == null ? null! : base.Visit(tree);
}