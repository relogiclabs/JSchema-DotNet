using RelogicLabs.JSchema.Antlr;
using RelogicLabs.JSchema.Collections;
using RelogicLabs.JSchema.Nodes;
using RelogicLabs.JSchema.Utilities;
using static RelogicLabs.JSchema.Antlr.JsonParser;
using static RelogicLabs.JSchema.Tree.TreeType;
using static RelogicLabs.JSchema.Tree.TreeHelper;

namespace RelogicLabs.JSchema.Tree;

internal sealed class JsonTreeVisitor : JsonParserBaseVisitor<JNode>
{
    private readonly Dictionary<JNode, JNode> _relations = new();
    private readonly RuntimeContext _runtime;

    public JsonTreeVisitor(RuntimeContext runtime)
        => _runtime = runtime;

    public override JNode VisitJson(JsonContext context)
        => new JRoot.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Value = Visit(context.valueNode())
        }.Build();

    public override JNode VisitObjectNode(ObjectNodeContext context)
        => new JObject.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Properties = ProcessProperties(context.propertyNode())
        }.Build();

    private IIndexMap<string,JProperty> ProcessProperties(PropertyNodeContext[] contexts)
    {
        var properties = contexts.Select(p => (JProperty) Visit(p)).ToList();
        return RequireUniqueness(properties, JSON_TREE).ToIndexMap().AsReadOnly();
    }

    public override JNode VisitPropertyNode(PropertyNodeContext context)
        => new JProperty.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Key = context.STRING().GetText()[1..^1],
            Value = Visit(context.valueNode())
        }.Build();

    public override JNode VisitArrayNode(ArrayNodeContext context)
        => new JArray.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Elements = context.valueNode().Select(Visit).ToList().AsReadOnly()
        }.Build();

    public override JNode VisitPrimitiveTrue(PrimitiveTrueContext context)
        => new JBoolean.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Value = true
        }.Build();

    public override JNode VisitPrimitiveFalse(PrimitiveFalseContext context)
        => new JBoolean.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Value = false
        }.Build();

    public override JNode VisitPrimitiveString(PrimitiveStringContext context)
        => new JString.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Value = context.STRING().GetText().ToEncoded()
        }.Build();

    public override JNode VisitPrimitiveInteger(PrimitiveIntegerContext context)
        => new JInteger.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Value = Convert.ToInt64(context.INTEGER().GetText())
        }.Build();

    public override JNode VisitPrimitiveFloat(PrimitiveFloatContext context)
        => new JFloat.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Value = Convert.ToDouble(context.FLOAT().GetText())
        }.Build();

    public override JNode VisitPrimitiveDouble(PrimitiveDoubleContext context)
        => new JDouble.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Value = Convert.ToDouble(context.DOUBLE().GetText())
        }.Build();

    public override JNode VisitPrimitiveNull(PrimitiveNullContext context)
        => new JNull.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime)
        }.Build();
}