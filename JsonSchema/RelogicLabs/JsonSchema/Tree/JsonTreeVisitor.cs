using RelogicLabs.JsonSchema.Antlr;
using RelogicLabs.JsonSchema.Collections;
using RelogicLabs.JsonSchema.Types;
using RelogicLabs.JsonSchema.Utilities;
using static RelogicLabs.JsonSchema.Message.ErrorCode;

namespace RelogicLabs.JsonSchema.Tree;

internal class JsonTreeVisitor : JsonParserBaseVisitor<JNode>
{
    private readonly Dictionary<JNode, JNode> _relations = new();
    private readonly RuntimeContext _runtime;

    public JsonTreeVisitor(RuntimeContext runtime)
        => _runtime = runtime;

    public override JNode VisitJson(JsonParser.JsonContext context)
        => new JRoot.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Value = Visit(context.value()),
        }.Build();

    public override JNode VisitValue(JsonParser.ValueContext context)
    {
        if(context.primitive() != null) return Visit(context.primitive());
        if(context.array() != null) return Visit(context.array());
        if(context.@object() != null) return Visit(context.@object());
        throw new InvalidOperationException("Invalid parser state");
    }

    public override JNode VisitObject(JsonParser.ObjectContext context)
        => new JObject.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Properties = ProcessProperties(context.property())
        }.Build();

    private IIndexMap<string,JProperty> ProcessProperties(JsonParser.PropertyContext[] contexts)
    {
        List<JProperty> properties = contexts.Select(p => (JProperty) Visit(p)).ToList();
        return JProperty.CheckForDuplicate(properties, PROP03).ToIndexMap().AsReadOnly();
    }

    public override JNode VisitProperty(JsonParser.PropertyContext context)
        => new JProperty.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Key = context.STRING().GetText()[1..^1],
            Value = Visit(context.value())
        }.Build();

    public override JNode VisitArray(JsonParser.ArrayContext context)
        => new JArray.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Elements = context.value().Select(Visit).ToList().AsReadOnly()
        }.Build();

    public override JNode VisitPrimitiveTrue(JsonParser.PrimitiveTrueContext context)
        => new JBoolean.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Value = true
        }.Build();

    public override JNode VisitPrimitiveFalse(JsonParser.PrimitiveFalseContext context)
        => new JBoolean.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Value = false
        }.Build();

    public override JNode VisitPrimitiveString(JsonParser.PrimitiveStringContext context)
        => new JString.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Value = context.STRING().GetText().ToEncoded()
        }.Build();

    public override JNode VisitPrimitiveInteger(JsonParser.PrimitiveIntegerContext context)
        => new JInteger.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Value = Convert.ToInt64(context.INTEGER().GetText())
        }.Build();

    public override JNode VisitPrimitiveFloat(JsonParser.PrimitiveFloatContext context)
        => new JFloat.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Value = Convert.ToDouble(context.FLOAT().GetText())
        }.Build();

    public override JNode VisitPrimitiveDouble(JsonParser.PrimitiveDoubleContext context)
        => new JDouble.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime),
            Value = Convert.ToDouble(context.DOUBLE().GetText())
        }.Build();

    public override JNode VisitPrimitiveNull(JsonParser.PrimitiveNullContext context)
        => new JNull.Builder
        {
            Relations = _relations,
            Context = new Context(context, _runtime)
        }.Build();
}