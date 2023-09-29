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
        => new JRoot(_relations)
        {
            Context = new Context(context, _runtime),
            Value = Visit(context.value()),
        }.Initialize();

    public override JNode VisitValue(JsonParser.ValueContext context)
    {
        if(context.primitive() != null) return Visit(context.primitive());
        if(context.array() != null) return Visit(context.array());
        if(context.@object() != null) return Visit(context.@object());
        throw new InvalidOperationException("Invalid parser state");
    }

    public override JNode VisitObject(JsonParser.ObjectContext context)
        => new JObject(_relations)
        {
            Context = new Context(context, _runtime),
            Properties = ProcessProperties(context.property())
        }.Initialize();

    private IIndexMap<string,JProperty> ProcessProperties(JsonParser.PropertyContext[] contexts)
    {
        List<JProperty> properties = contexts.Select(p => (JProperty) Visit(p)).ToList();
        return JProperty.CheckForDuplicate(properties, PROP03).ToIndexMap();
    }

    public override JNode VisitProperty(JsonParser.PropertyContext context)
        => new JProperty(_relations)
        {
            Context = new Context(context, _runtime),
            Key = context.STRING().GetText()[1..^1],
            Value = Visit(context.value())
        }.Initialize();

    public override JNode VisitArray(JsonParser.ArrayContext context)
        => new JArray(_relations)
        {
            Context = new Context(context, _runtime),
            Elements = context.value().Select(Visit).ToList().AsReadOnly()
        }.Initialize();

    public override JNode VisitPrimitiveTrue(JsonParser.PrimitiveTrueContext context)
        => new JBoolean(_relations)
        {
            Context = new Context(context, _runtime),
            Value = true
        }.Initialize();

    public override JNode VisitPrimitiveFalse(JsonParser.PrimitiveFalseContext context)
        => new JBoolean(_relations)
        {
            Context = new Context(context, _runtime),
            Value = false
        }.Initialize();

    public override JNode VisitPrimitiveString(JsonParser.PrimitiveStringContext context)
        => new JString(_relations)
        {
            Context = new Context(context, _runtime),
            Value = context.STRING().GetText().ToEncoded()
        }.Initialize();

    public override JNode VisitPrimitiveInteger(JsonParser.PrimitiveIntegerContext context)
        => new JInteger(_relations)
        {
            Context = new Context(context, _runtime),
            Value = Convert.ToInt64(context.INTEGER().GetText())
        }.Initialize();

    public override JNode VisitPrimitiveFloat(JsonParser.PrimitiveFloatContext context)
        => new JFloat(_relations)
        {
            Context = new Context(context, _runtime),
            Value = Convert.ToDouble(context.FLOAT().GetText())
        }.Initialize();

    public override JNode VisitPrimitiveDouble(JsonParser.PrimitiveDoubleContext context)
        => new JDouble(_relations)
        {
            Context = new Context(context, _runtime),
            Value = Convert.ToDouble(context.DOUBLE().GetText())
        }.Initialize();

    public override JNode VisitPrimitiveNull(JsonParser.PrimitiveNullContext context)
        => new JNull(_relations) { Context = new Context(context, _runtime) }
            .Initialize();
}