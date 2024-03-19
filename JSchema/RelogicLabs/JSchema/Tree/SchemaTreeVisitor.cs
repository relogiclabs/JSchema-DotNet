using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using RelogicLabs.JSchema.Antlr;
using RelogicLabs.JSchema.Collections;
using RelogicLabs.JSchema.Engine;
using RelogicLabs.JSchema.Functions;
using RelogicLabs.JSchema.Nodes;
using RelogicLabs.JSchema.Utilities;
using static RelogicLabs.JSchema.Tree.TreeType;
using static RelogicLabs.JSchema.Tree.TreeHelper;

namespace RelogicLabs.JSchema.Tree;

internal sealed class SchemaTreeVisitor : SchemaParserBaseVisitor<JNode>
{
    private readonly Dictionary<JNode, JNode> _relations = new();
    private readonly RuntimeContext _runtime;
    private readonly ParseTreeProperty<Evaluator> _scripts;

    public SchemaTreeVisitor(ScriptTreeVisitor scriptVisitor)
    {
        _runtime = scriptVisitor.Runtime;
        _scripts = scriptVisitor.Scripts;
    }

    public override JNode VisitCompleteSchema(SchemaParser.CompleteSchemaContext context)
        => new JRoot.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Title = (JTitle) Visit(context.titleNode()),
            Version = (JVersion) Visit(context.versionNode()),
            Imports = ProcessImports(context.importNode()).AsReadOnly(),
            Pragmas = context.pragmaNode().Select(p => (JPragma) Visit(p)).ToList().AsReadOnly(),
            Definitions = context.defineNode().Select(d => (JDefinition) Visit(d)).ToList().AsReadOnly(),
            Scripts = context.scriptNode().Select(s => (JScript) Visit(s)).ToList().AsReadOnly(),
            Value = Visit(context.schemaMain())
        }.Build();

    public override JNode VisitShortSchema(SchemaParser.ShortSchemaContext context)
        => new JRoot.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Imports = ProcessImports(Array.Empty<SchemaParser.ImportNodeContext>()),
            Value = Visit(context.validatorNode())
        }.Build();

    private List<JImport> ProcessImports(SchemaParser.ImportNodeContext[] contexts)
    {
        _runtime.Functions.AddClass(typeof(CoreFunctions).FullName!);
        return contexts.Select(i => (JImport) Visit(i)).ToList();
    }

    public override JNode VisitSchemaMain(SchemaParser.SchemaMainContext context)
        => Visit(context.validatorNode());

    public override JNode VisitTitleNode(SchemaParser.TitleNodeContext context)
        => new JTitle.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Title = context.STRING().GetText()
        }.Build();

    public override JNode VisitVersionNode(SchemaParser.VersionNodeContext context)
        => new JVersion.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Version = context.STRING().GetText()
        }.Build();

    public override JNode VisitImportNode(SchemaParser.ImportNodeContext context)
    {
        var import = new JImport.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            ClassName = context.FULL_IDENTIFIER().Select(static n => n.GetText()).Join(",")
        }.Build();
        return _runtime.Functions.AddClass(import);
    }

    public override JNode VisitPragmaNode(SchemaParser.PragmaNodeContext context)
    {
        var pragma = new JPragma.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Name = context.FULL_IDENTIFIER().GetText(),
            Value = (JPrimitive) Visit(context.primitiveNode())
        }.Build();
        return _runtime.Pragmas.AddPragma(pragma);
    }

    public override JNode VisitDefineNode(SchemaParser.DefineNodeContext context)
    {
        var definition = new JDefinition.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Alias = (JAlias) Visit(context.aliasNode()),
            Validator = (JValidator) Visit(context.validatorMain())
        }.Build();
        return _runtime.AddDefinition(definition);
    }

    public override JNode VisitScriptNode(SchemaParser.ScriptNodeContext context)
    {
        var source = context.Start.InputStream.GetText(new Interval(
            context.Start.StartIndex,
            context.Stop.StopIndex));
        return new JScript.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Evaluator = _scripts.Get(context),
            Source = source
        }.Build();
    }

    public override JNode VisitAliasNode(SchemaParser.AliasNodeContext context)
        => new JAlias.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Name = context.ALIAS().GetText()
        }.Build();

    public override JNode VisitValidatorMain(SchemaParser.ValidatorMainContext context)
        => new JValidator.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Value = Visit(context.valueNode()),
            Functions = context.functionNode().Select(ctx => (JFunction) Visit(ctx))
                .ToList().AsReadOnly(),
            DataTypes = context.datatypeNode().Select(ctx => (JDataType) Visit(ctx))
                .ToList().AsReadOnly(),
            Receivers = context.receiverNode().Select(ctx => (JReceiver) Visit(ctx))
                .ToList().AsReadOnly(),
            Optional = context.OPTIONAL() != null
        }.Build();

    public override JNode VisitObjectNode(SchemaParser.ObjectNodeContext context)
        => new JObject.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Properties = ProcessProperties(context.propertyNode())
        }.Build();

    private  IIndexMap<string,JProperty> ProcessProperties(SchemaParser.PropertyNodeContext[] contexts)
    {
        List<JProperty> properties = contexts.Select(p => (JProperty) Visit(p)).ToList();
        return RequireUniqueness(properties, SCHEMA_TREE).ToIndexMap().AsReadOnly();
    }

    public override JNode VisitPropertyNode(SchemaParser.PropertyNodeContext context)
        => new JProperty.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Key = context.STRING().GetText()[1..^1],
            Value = Visit(context.validatorNode())
        }.Build();

    public override JNode VisitArrayNode(SchemaParser.ArrayNodeContext context)
        => new JArray.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Elements = context.validatorNode().Select(Visit).ToList().AsReadOnly()
        }.Build();

    public override JNode VisitDatatypeNode(SchemaParser.DatatypeNodeContext context)
        => new JDataType.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            JsonType = JsonType.From(context.DATATYPE()),
            Nested = context.STAR() != null,
            Alias = (JAlias) Visit(context.aliasNode())
        }.Build();

    public override JNode VisitFunctionNode(SchemaParser.FunctionNodeContext context)
        => new JFunction.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Name = context.FUNCTION().GetText(),
            Nested = context.STAR() != null,
            Arguments = context.argumentNode().Select(Visit).ToList().AsReadOnly()
        }.Build();

    public override JNode VisitReceiverNode(SchemaParser.ReceiverNodeContext context)
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