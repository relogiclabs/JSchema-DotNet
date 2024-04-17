using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using RelogicLabs.JSchema.Antlr;
using RelogicLabs.JSchema.Collections;
using RelogicLabs.JSchema.Engine;
using RelogicLabs.JSchema.Nodes;
using RelogicLabs.JSchema.Utilities;
using static RelogicLabs.JSchema.Antlr.SchemaParser;
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

    public override JNode VisitCompleteSchema(CompleteSchemaContext context)
        => new JRoot.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Title = (JTitle) Visit(context.titleNode()),
            Version = (JVersion) Visit(context.versionNode()),
            Imports = context.importNode().Select(i => (JImport) Visit(i)).ToList().AsReadOnly(),
            Pragmas = context.pragmaNode().Select(p => (JPragma) Visit(p)).ToList().AsReadOnly(),
            Definitions = context.defineNode().Select(d => (JDefinition) Visit(d)).ToList().AsReadOnly(),
            Scripts = context.scriptNode().Select(s => (JScript) Visit(s)).ToList().AsReadOnly(),
            Value = Visit(context.schemaCoreNode())
        }.Build();

    public override JNode VisitShortSchema(ShortSchemaContext context)
        => new JRoot.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Imports = Array.Empty<JImport>(),
            Value = Visit(context.validatorNode())
        }.Build();

    public override JNode VisitTitleNode(TitleNodeContext context)
        => new JTitle.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Title = context.S_STRING().GetText()
        }.Build();

    public override JNode VisitVersionNode(VersionNodeContext context)
        => new JVersion.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Version = context.S_STRING().GetText()
        }.Build();

    public override JNode VisitImportNode(ImportNodeContext context)
    {
        var import = new JImport.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            ClassName = context.S_GENERAL_ID().Select(static n => n.GetText()).Join(",")
        }.Build();
        return _runtime.Functions.AddImport(import);
    }

    public override JNode VisitPragmaNode(PragmaNodeContext context)
    {
        var pragma = new JPragma.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Name = context.S_GENERAL_ID().GetText(),
            Value = (JPrimitive) Visit(context.primitiveNode())
        }.Build();
        return _runtime.Pragmas.AddPragma(pragma);
    }

    public override JNode VisitDefineNode(DefineNodeContext context)
    {
        var definition = new JDefinition.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Alias = (JAlias) Visit(context.aliasNode()),
            Validator = (JValidator) Visit(context.validatorMainNode())
        }.Build();
        return _runtime.AddDefinition(definition);
    }

    public override JNode VisitScriptNode(ScriptNodeContext context)
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

    public override JNode VisitAliasNode(AliasNodeContext context)
        => new JAlias.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Name = context.S_ALIAS().GetText()
        }.Build();

    public override JNode VisitValidatorMainNode(ValidatorMainNodeContext context)
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
            Optional = context.S_OPTIONAL() != null
        }.Build();

    public override JNode VisitObjectNode(ObjectNodeContext context)
        => new JObject.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Properties = ProcessProperties(context.propertyNode())
        }.Build();

    private  IIndexMap<string,JProperty> ProcessProperties(PropertyNodeContext[] contexts)
    {
        var properties = contexts.Select(p => (JProperty) Visit(p)).ToList();
        return RequireUniqueness(properties, SCHEMA_TREE).ToIndexMap().AsReadOnly();
    }

    public override JNode VisitPropertyNode(PropertyNodeContext context)
        => new JProperty.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Key = context.S_STRING().GetText()[1..^1],
            Value = Visit(context.validatorNode())
        }.Build();

    public override JNode VisitArrayNode(ArrayNodeContext context)
        => new JArray.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Elements = context.validatorNode().Select(Visit).ToList().AsReadOnly()
        }.Build();

    public override JNode VisitDatatypeNode(DatatypeNodeContext context)
        => new JDataType.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            JsonType = JsonType.From(context.S_DATATYPE()),
            Nested = context.S_STAR() != null,
            Alias = (JAlias) Visit(context.aliasNode())
        }.Build();

    public override JNode VisitFunctionNode(FunctionNodeContext context)
        => new JFunction.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Name = context.S_FUNCTION().GetText(),
            Nested = context.S_STAR() != null,
            Arguments = context.argumentNode().Select(Visit).ToList().AsReadOnly()
        }.Build();

    public override JNode VisitReceiverNode(ReceiverNodeContext context)
        => new JReceiver.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Name = context.S_RECEIVER().GetText()
        }.Build();

    public override JNode VisitTrueNode(TrueNodeContext context)
        => new JBoolean.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Value = true
        }.Build();

    public override JNode VisitFalseNode(FalseNodeContext context)
        => new JBoolean.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Value = false
        }.Build();

    public override JNode VisitStringNode(StringNodeContext context)
        => new JString.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Value = context.S_STRING().GetText().ToEncoded()
        }.Build();

    public override JNode VisitIntegerNode(IntegerNodeContext context)
        => new JInteger.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Value = Convert.ToInt64(context.S_INTEGER().GetText())
        }.Build();

    public override JNode VisitFloatNode(FloatNodeContext context)
        => new JFloat.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Value = Convert.ToDouble(context.S_FLOAT().GetText())
        }.Build();

    public override JNode VisitDoubleNode(DoubleNodeContext context)
        => new JDouble.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Value = Convert.ToDouble(context.S_DOUBLE().GetText())
        }.Build();

    public override JNode VisitNullNode(NullNodeContext context)
        => new JNull.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime)
        }.Build();

    public override JNode VisitUndefinedNode(UndefinedNodeContext context)
        => new JUndefined.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime)
        }.Build();

    public override JNode Visit(IParseTree? tree)
        => tree == null ? null! : base.Visit(tree);
}